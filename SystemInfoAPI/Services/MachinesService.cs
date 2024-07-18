using System.Data.SqlClient;
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

                foreach (DriveModel drive in machine.Drives)
                {
                    // Set new machineId on drive and insert
                    drive.MachineId = updatedMachine.Id;
                    DriveModel updatedDrive = await drivesRepository.InsertAsync(drive, connection, transaction);

                    int historyDriveId = await drivesRepository.InsertHistoryAsync(drive, connection, transaction);

                    if (drive.IsSystemDrive && drive.Os != null)
                    {
                        // Set new driveId on OS and insert
                        drive.Os.DriveId = updatedDrive.Id;
                        OsModel updatedOs = await osRepository.InsertAsync(drive.Os, connection, transaction);
                        updatedDrive.Os = updatedOs;

                        await osRepository.InsertHistoryAsync(drive.Os, connection, transaction, historyDriveId);
                    }

                    foreach (ApplicationModel app in drive.AppList)
                    {
                        // Set driveId on app and insert
                        app.DriveId = updatedDrive.Id;
                        await appRepository.InsertAsync(app, connection, transaction);

                        await appRepository.InsertHistoryAsync(app, connection, transaction, historyDriveId);
                    } 

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
                    (DriveModel updatedDrive, existingDrivesDict) = 
                        await ProcessDriveAsync(drive, existingDrivesDict, connection, transaction);

                    updatedDrivesList.Add(updatedDrive);
                    int historyDriveId = await drivesRepository.InsertHistoryAsync(drive, connection, transaction);

                    // Process drive's OS
                    if (drive.IsSystemDrive && drive.Os != null)
                    {
                        await ProcessOsAsync(drive.Os, existingDrive, updatedDrive.Id, historyDriveId, connection, transaction);
                    }

                    // Process drive's Apps
                    if (drive.AppList != null)
                    {
                        await ProcessAppsAsync(drive, existingDrive, historyDriveId, connection, transaction);
                    }
                }

                await DeleteRemainingDrivesAsync(existingDrivesDict, connection, transaction);

                return updatedDrivesList;
            }

            async Task<(DriveModel, Dictionary<string, DriveModel>)> ProcessDriveAsync(DriveModel drive, Dictionary<string, DriveModel> existingDrivesDict, SqlConnection connection, SqlTransaction transaction)
            {
                drive.MachineId = machine.Id;
                DriveModel updatedDrive;

                // If one of the drives in the database has this drive's serial number...
                if (existingDrivesDict.ContainsKey(drive.SerialNumber))
                {
                    // ...update it and remove it from the dictionary (so we can tell it has been processed)...
                    updatedDrive = await drivesRepository.UpdateAsync(drive, connection, transaction);
                    existingDrivesDict.Remove(drive.SerialNumber);
                }
                else
                {
                    // ...otherwise create a new one.
                    updatedDrive = await drivesRepository.InsertAsync(drive, connection, transaction);
                    ConsoleUtils.LogDriveCreation(drive.Name, drive.SerialNumber);
                }

                drive.Id = updatedDrive.Id;
                return (updatedDrive, existingDrivesDict);
            }

            async Task ProcessOsAsync(OsModel os, DriveModel? existingDrive, int driveId, int historyDriveId, SqlConnection connection, SqlTransaction transaction)
            {
                os.DriveId = driveId;
                if (existingDrive != null && existingDrive.Os != null && existingDrive.Os.Id != 0)
                {                    
                    OsModel updatedOs = await osRepository.UpdateAsync(os, connection, transaction);                   
                    os.Id = updatedOs.Id;
                }
                else
                {
                    OsModel updatedOs = await osRepository.InsertAsync(os, connection, transaction);
                    ConsoleUtils.LogOsCreation(os.DriveId, machine.Id);
                    os.Id = updatedOs.Id;
                }

                // Create OS history
                await osRepository.InsertHistoryAsync(os, connection, transaction, historyDriveId);
            }

            async Task ProcessAppsAsync(DriveModel drive, DriveModel? existingDrive, int historyDriveId, SqlConnection connection, SqlTransaction transaction)
            {
                // If the drive already exists we can do comparisons to update its applications
                if (existingDrive != null)
                {
                    var existingAppsDict = existingDrive.AppList.ToDictionary(app => app.Id) ?? [];

                    foreach (ApplicationModel app in drive.AppList)
                    {
                        app.DriveId = drive.Id;

                        // Check if the app already has a relation with this drive
                        // If not, create instead of update
                        if (existingAppsDict.ContainsKey(app.Id))
                        {
                            await appRepository.UpdateAsync(app, connection, transaction);
                            existingAppsDict.Remove(app.Id);
                        }
                        else
                        {
                            ConsoleUtils.LogAppCreation(app.Name, app.Id, app.DriveId);
                            await appRepository.InsertAsync(app, connection, transaction);
                        }
                        await appRepository.InsertHistoryAsync(app, connection, transaction, historyDriveId);
                    } 

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
