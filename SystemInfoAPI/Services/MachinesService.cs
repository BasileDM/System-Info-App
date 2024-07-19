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
        public async Task<MachineModel> InsertFullMachineAsync(MachineModel machine, DateTime requestDate)
        {
            return await MakeTransactionAsync(async (connection, transaction) =>
            {
                // Insert machine and get new ID
                machine.CreationDate = requestDate;
                MachineModel updatedMachine = await machinesRepository.InsertAsync(machine, connection, transaction);
                List<DriveModel> updatedDrivesList = [];

                foreach (DriveModel drive in updatedMachine.Drives)
                {
                    // Insert drive
                    drive.MachineId = updatedMachine.Id;
                    drive.CreationDate = requestDate;

                    DriveModel updatedDrive = await drivesRepository.InsertAsync(drive, connection, transaction);
                    int driveHistoryId = await drivesRepository.InsertHistoryAsync(updatedDrive, connection, transaction);

                    // Insert drive's OS
                    if (updatedDrive.IsSystemDrive && updatedDrive.Os != null)
                    {
                        updatedDrive.Os.DriveId = updatedDrive.Id;
                        updatedDrive.Os.CreationDate = requestDate;
                        updatedDrive.Os = await osRepository.InsertAsync(updatedDrive.Os, connection, transaction);

                        await osRepository.InsertHistoryAsync(updatedDrive.Os, connection, transaction, driveHistoryId);
                    }

                    // Insert drive's apps
                    foreach (ApplicationModel app in updatedDrive.AppList)
                    {
                        app.DriveId = updatedDrive.Id;
                        app.CreationDate = requestDate;
                    }

                    await appRepository.InsertListAsync(updatedDrive.AppList, connection, transaction);
                    await appRepository.InsertHistoryListAsync(updatedDrive.AppList, connection, transaction, driveHistoryId);

                    updatedDrivesList.Add(updatedDrive);
                }

                updatedMachine.Drives = updatedDrivesList;
                return updatedMachine;
            });
        }
        /// <summary>
        ///   Handles the business logic to update a machine in the database.
        ///   This method uses a transaction to ensure all inserts are successful or rolled back in case of an error.
        /// </summary>
        /// <param name="machine">The <see cref="MachineModel"/> object without IDs to handle.</param>
        /// <returns>
        ///   A new <see cref="MachineModel"/> object with the created IDs from the database.
        /// </returns>
        /// <remarks>
        ///   This methods uses the <see cref="GetByIdAsync(int)"/> method to get the current state of the machine from the DB. 
        ///   This method calls <see cref="Database.MakeTransactionAsync{T}"/> to handle the transaction and ensure data consistency.
        /// </remarks>
        public async Task<MachineModel> UpdateFullMachineAsync(MachineModel machine, DateTime requestDate)
        {
            return await MakeTransactionAsync(async (connection, transaction) =>
            {
                machine.CreationDate = requestDate;
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
                    // Get drive's state from the DB for comparison
                    var existingDrive = existingMachine.Drives.FirstOrDefault(ed => ed.SerialNumber == drive.SerialNumber);

                    // Process drive, OS and apps
                    (DriveModel updatedDrive, existingDrivesDict, int driveHistoryId) =
                        await ProcessDriveAsync(drive, existingDrivesDict, connection, transaction);

                    updatedDrive.Os = await ProcessOsAsync(updatedDrive, existingDrive, driveHistoryId, connection, transaction);

                    await ProcessAppsAsync(updatedDrive, existingDrive, driveHistoryId, connection, transaction);

                    updatedDrivesList.Add(updatedDrive);
                }

                await DeleteRemainingDrivesAsync(existingDrivesDict, connection, transaction);

                return updatedDrivesList;
            }

            async Task<(DriveModel, Dictionary<string, DriveModel>, int)> ProcessDriveAsync(DriveModel drive, Dictionary<string, DriveModel> existingDrivesDict, SqlConnection connection, SqlTransaction transaction)
            {
                drive.MachineId = machine.Id;
                drive.CreationDate = requestDate;
                DriveModel updatedDrive;

                // If the drive already exists, it should be updated...
                if (existingDrivesDict.ContainsKey(drive.SerialNumber))
                {
                    updatedDrive = await drivesRepository.UpdateAsync(drive, connection, transaction);

                    // Remove it from the dictionary so we can tell it has been processed.
                    existingDrivesDict.Remove(updatedDrive.SerialNumber);
                }

                // ...otherwise create a new one.
                else
                {
                    updatedDrive = await drivesRepository.InsertAsync(drive, connection, transaction);
                    ConsoleUtils.LogDriveCreation(updatedDrive.Name, updatedDrive.SerialNumber);
                }

                int historyDriveId = await drivesRepository.InsertHistoryAsync(updatedDrive, connection, transaction);

                return (updatedDrive, existingDrivesDict, historyDriveId);
            }

            async Task<OsModel?> ProcessOsAsync(DriveModel drive, DriveModel? existingDrive, int driveHistoryId, SqlConnection connection, SqlTransaction transaction)
            {
                if (drive.IsSystemDrive && drive.Os != null)
                {
                    drive.Os.DriveId = drive.Id;
                    drive.Os.CreationDate = requestDate;
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
                    await osRepository.InsertHistoryAsync(updatedOs, connection, transaction, driveHistoryId);
                    return updatedOs;
                }
                else
                {
                    return null;
                }
            }

            async Task ProcessAppsAsync(DriveModel drive, DriveModel? existingDrive, int driveHistoryId, SqlConnection connection, SqlTransaction transaction)
            {
                List<ApplicationModel> appsToUpdate = [];
                List<ApplicationModel> appsToInsert = [];
                List<ApplicationModel> appsToDelete = [];

                // If the drive already exists we can do comparisons to update its applications
                if (existingDrive != null)
                {
                    var existingAppsDict = existingDrive.AppList.ToDictionary(app => app.Id) ?? [];

                    if (drive.AppList.Count > 0)
                    {
                        foreach (ApplicationModel app in drive.AppList)
                        {
                            app.DriveId = drive.Id;
                            app.CreationDate = requestDate;

                            // If the app already exists, update and remove from dictionary (so we can tell it has been processed)
                            if (existingAppsDict.ContainsKey(app.Id))
                            {
                                appsToUpdate.Add(app);
                                existingAppsDict.Remove(app.Id);
                            }
                            // If not it should be created
                            else
                            {
                                appsToInsert.Add(app);
                                ConsoleUtils.LogAppCreation(app.Name, app.Id, app.DriveId);
                            }
                        }
                        if (appsToUpdate.Count > 0) await appRepository.UpdateListAsync(appsToUpdate, connection, transaction);
                        if (appsToInsert.Count > 0) await appRepository.InsertListAsync(appsToInsert, connection, transaction);
                        await appRepository.InsertHistoryListAsync(drive.AppList, connection, transaction, driveHistoryId);
                    }

                    // Delete remaining old apps not in the new list
                    foreach (var appToDelete in existingAppsDict.Values)
                    {
                        ConsoleUtils.LogAppDeletion(appToDelete.Name, appToDelete.Id, existingDrive.Id);
                        appsToDelete.Add(appToDelete);
                    }
                    await appRepository.DeleteDriveRelationListAsync(appsToDelete, connection, transaction);
                }
                // If the existing drive is null, the current drive is a new one, and we can create all the apps
                else if (drive.AppList.Count > 0)
                {
                    foreach (var app in drive.AppList)
                    {
                        app.DriveId = drive.Id;
                        app.CreationDate = requestDate;
                        ConsoleUtils.LogAppCreation(app.Name, app.Id, app.DriveId);

                    }
                    await appRepository.InsertListAsync(drive.AppList, connection, transaction);
                    await appRepository.InsertHistoryListAsync(drive.AppList, connection, transaction, driveHistoryId);
                }
            }

            async Task DeleteRemainingDrivesAsync(Dictionary<string, DriveModel> existingDrivesDict, SqlConnection connection, SqlTransaction transaction)
            {
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
