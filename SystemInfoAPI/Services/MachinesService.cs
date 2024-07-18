﻿using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;
using SystemInfoApi.Repositories;
using SystemInfoApi.Utilities;

namespace SystemInfoApi.Services
{
    public class MachinesService(MachinesRepository machinesRepository, DrivesRepository drivesRepository,
        OsRepository osRepository, ApplicationsRepository appRepository, IConfiguration config, IWebHostEnvironment env) : Database(config, env)
    {
        /// <summary>
        /// Handles the business logic to create a new full machine in the database.
        /// This method uses a transaction to ensure all inserts are successful or rolled back in case of an error.
        /// </summary>
        /// <param name="machine">The <see cref="MachineModel"/> object without IDs to handle.</param>
        /// <returns>
        ///   A new <see cref="MachineModel"/> object with the created IDs from the database.
        /// </returns>
        /// <remarks>
        /// This method calls <see cref="Database.MakeTransactionAsync{T}"/> to handle the transaction and ensure data consistency.
        /// </remarks>
        public async Task<MachineModel> InsertFullMachineAsync(MachineModel machine)
        {
            return await MakeTransactionAsync(async (connection, transaction) =>
            {
                // Insert machine and get new ID
                MachineModel updatedMachine = await machinesRepository.InsertAsync(machine, connection, transaction);
                List<DriveModel> updatedDrivesList = [];

                foreach (DriveModel drive in updatedMachine.Drives)
                {
                    // Set new machineId on drive and insert
                    drive.MachineId = updatedMachine.Id;
                    DriveModel updatedDrive = await drivesRepository.InsertAsync(drive, connection, transaction);

                    int historyDriveId = await drivesRepository.InsertHistoryAsync(updatedDrive, connection, transaction);

                    if (updatedDrive.IsSystemDrive && updatedDrive.Os != null)
                    {
                        // Set new driveId on OS and insert
                        updatedDrive.Os.DriveId = updatedDrive.Id;
                        OsModel updatedOs = await osRepository.InsertAsync(updatedDrive.Os, connection, transaction);
                        updatedDrive.Os = updatedOs;

                        await osRepository.InsertHistoryAsync(updatedDrive.Os, connection, transaction, historyDriveId);
                    }

                    foreach (ApplicationModel app in updatedDrive.AppList)
                    {
                        // Set driveId on app and insert
                        app.DriveId = updatedDrive.Id;
                        app.CreationDate = DateTime.Now.ToLocalTime();
                        //await appRepository.InsertAsync(app, connection, transaction);

                        await appRepository.InsertHistoryAsync(app, connection, transaction, historyDriveId);
                    }
                    await appRepository.InsertListAsync(updatedDrive.AppList, connection, transaction);

                    updatedDrivesList.Add(updatedDrive);
                }
                updatedMachine.Drives = updatedDrivesList;
                return updatedMachine;
            });
        }
        public async Task<MachineModel> UpdateFullMachineAsync(MachineModel machine)
        {
            return await MakeTransactionAsync(async (connection, transaction) =>
            {
                await machinesRepository.UpdateAsync(machine, connection, transaction);

                // Get the current machine from the DB for comparison.
                MachineModel existingMachine = await machinesRepository.GetByIdAsync(machine.Id, connection, transaction);

                List<DriveModel> updatedDrivesList = await ProcessAllDrivesAsync(existingMachine, connection, transaction);

                machine.Drives = updatedDrivesList;

                return machine;
            });

            async Task<List<DriveModel>> ProcessAllDrivesAsync(MachineModel existingMachine, SqlConnection connection, SqlTransaction transaction)
            {
                List<DriveModel> updatedDrivesList = [];
                var existingDrivesDict = existingMachine.Drives.ToDictionary(d => d.SerialNumber); // Serial as alt identifier.

                foreach (DriveModel drive in machine.Drives)
                {
                    var existingDrive = existingMachine.Drives.FirstOrDefault(ed => ed.SerialNumber == drive.SerialNumber);

                    // Process the drive
                    (DriveModel updatedDrive, existingDrivesDict, int historyDriveId) = 
                        await ProcessDriveAsync(drive, existingDrivesDict, connection, transaction);

                    // Process drive's OS
                    updatedDrive.Os = await ProcessOsAsync(updatedDrive, existingDrive, historyDriveId, connection, transaction);

                    // Process drive's Apps
                    await ProcessAppsAsync(updatedDrive, existingDrive, historyDriveId, connection, transaction);

                    updatedDrivesList.Add(updatedDrive);
                }

                await DeleteRemainingDrivesAsync(existingDrivesDict, connection, transaction);

                return updatedDrivesList;
            }

            async Task<(DriveModel, Dictionary<string, DriveModel>, int)> ProcessDriveAsync(DriveModel drive, Dictionary<string, DriveModel> existingDrivesDict, SqlConnection connection, SqlTransaction transaction)
            {
                drive.MachineId = machine.Id;
                DriveModel updatedDrive;

                // If one of the drives in the database has this drive's serial number...
                if (existingDrivesDict.ContainsKey(drive.SerialNumber))
                {
                    // ...update it and remove it from the dictionary (so we can tell it has been processed)...
                    updatedDrive = await drivesRepository.UpdateAsync(drive, connection, transaction);
                    existingDrivesDict.Remove(updatedDrive.SerialNumber);
                }
                else
                {
                    // ...otherwise create a new one.
                    updatedDrive = await drivesRepository.InsertAsync(drive, connection, transaction);
                    ConsoleUtils.LogDriveCreation(updatedDrive.Name, updatedDrive.SerialNumber);
                }

                int historyDriveId = await drivesRepository.InsertHistoryAsync(updatedDrive, connection, transaction);

                return (updatedDrive, existingDrivesDict, historyDriveId);
            }

            async Task<OsModel?> ProcessOsAsync(DriveModel drive, DriveModel? existingDrive, int historyDriveId, SqlConnection connection, SqlTransaction transaction)
            {
                if (drive.IsSystemDrive && drive.Os != null)
                {
                    drive.Os.DriveId = drive.Id;
                    OsModel updatedOs;

                    if (existingDrive != null && existingDrive.Os != null && existingDrive.Os.Id != 0)
                    {
                        updatedOs = await osRepository.UpdateAsync(drive.Os, connection, transaction);
                    }
                    else
                    {
                        updatedOs = await osRepository.InsertAsync(drive.Os, connection, transaction);
                        ConsoleUtils.LogOsCreation(drive.Os.DriveId, machine.Id);
                    }

                    // Create OS history
                    await osRepository.InsertHistoryAsync(updatedOs, connection, transaction, historyDriveId);
                    return updatedOs;
                } 
                else
                {
                    return null;
                }
            }

            async Task ProcessAppsAsync(DriveModel drive, DriveModel? existingDrive, int historyDriveId, SqlConnection connection, SqlTransaction transaction)
            {
                // If the drive already exists we can do comparisons to update its applications
                if (existingDrive != null)
                {
                    var existingAppsDict = existingDrive.AppList.ToDictionary(app => app.Id) ?? [];

                    List<ApplicationModel> appsToUpdate = [];
                    List<ApplicationModel> appsToInsert = [];
                    List<ApplicationModel> appsToDelete = [];

                    foreach (ApplicationModel app in drive.AppList)
                    {
                        app.DriveId = drive.Id;

                        // Check if the app already has a relation with this drive
                        // If not, create instead of update
                        if (existingAppsDict.ContainsKey(app.Id))
                        {
                            //await appRepository.UpdateAsync(app, connection, transaction);
                            appsToUpdate.Add(app);
                            existingAppsDict.Remove(app.Id);
                        }
                        else
                        {
                            ConsoleUtils.LogAppCreation(app.Name, app.Id, app.DriveId);
                            appsToInsert.Add(app);
                            //await appRepository.InsertAsync(app, connection, transaction);
                        }

                        //await appRepository.InsertHistoryAsync(app, connection, transaction, historyDriveId);
                    }
                    if (appsToInsert.Count > 0) await appRepository.InsertListAsync(appsToInsert, connection, transaction);
                    if (appsToUpdate.Count > 0) await appRepository.UpdateListAsync(appsToUpdate, connection, transaction);
                    await appRepository.InsertHistoryListAsync(drive.AppList, connection, transaction, historyDriveId);

                    // Delete remaining old apps not in the new list
                    foreach (var appToDelete in existingAppsDict.Values)
                    {
                        ConsoleUtils.LogAppDeletion(appToDelete.Name, appToDelete.Id, existingDrive.Id);
                        await appRepository.DeleteDriveRelationAsync(appToDelete.Id, existingDrive.Id, connection, transaction);
                    } 
                }
                // If the existing drive is null, the current drive is a new one, and we can create all the apps
                else
                {
                    foreach (var app in drive.AppList)
                    {
                        app.DriveId = drive.Id;
                        ConsoleUtils.LogAppCreation(app.Name, app.Id, app.DriveId);
                        await appRepository.InsertAsync(app, connection, transaction);

                        // Create app history
                        await appRepository.InsertHistoryAsync(app, connection, transaction, historyDriveId);
                    }
                }
            }

            async Task DeleteRemainingDrivesAsync(Dictionary<string, DriveModel> existingDrivesDict, SqlConnection connection, SqlTransaction transaction)
            {
                // Delete remaining leftover drives, which are not in the machine any more
                foreach (var driveToDelete in existingDrivesDict.Values)
                {
                    ConsoleUtils.LogDriveDeletion(driveToDelete.Name, driveToDelete.Id, driveToDelete.SerialNumber);
                    await drivesRepository.DeleteAsync(driveToDelete.Id, connection, transaction);
                }
            }
        }
        public async Task<MachineModel> GetByIdAsync(int machineId)
        {
            return await MakeTransactionAsync(async (connection, transaction) =>
            {
                return await machinesRepository.GetByIdAsync(machineId, connection, transaction);
            });
        }
        public async Task<List<MachineModel>> GetAllAsync()
        {
            return await MakeTransactionAsync(async (connection, transaction) =>
            {
                return await machinesRepository.GetAllAsync(connection, transaction);
            });
        }
    }
}
