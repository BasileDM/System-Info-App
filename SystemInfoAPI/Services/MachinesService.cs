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

                    // Create drive history
                    int historyDriveId = await drivesRepository.InsertHistoryAsync(drive, connection, transaction);

                    if (drive.IsSystemDrive && drive.Os != null)
                    {
                        // Set new driveId on OS and insert
                        drive.Os.DriveId = updatedDrive.Id;
                        OsModel updatedOs = await osRepository.InsertAsync(drive.Os, connection, transaction);
                        updatedDrive.Os = updatedOs;

                        // Create OS history
                        await osRepository.InsertHistoryAsync(drive.Os, connection, transaction, historyDriveId);
                    }

                    foreach (ApplicationModel app in drive.AppList)
                    {
                        app.DriveId = updatedDrive.Id;
                        await appRepository.InsertAsync(app, connection, transaction);

                        // Create app history
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

                List<DriveModel> updatedDrivesList = await ProcessDrivesListAsync(machine, existingMachine, connection, transaction);

                machine.Drives = updatedDrivesList;

                return machine;
            });

            async Task<List<DriveModel>> ProcessDrivesListAsync(MachineModel machine, MachineModel existingMachine, SqlConnection connection, SqlTransaction transaction)
            {
                List<DriveModel> updatedDrivesList = [];
                var existingDrivesDict = existingMachine.Drives.ToDictionary(d => d.SerialNumber); // Serial as alt identifier.

                foreach (DriveModel drive in machine.Drives)
                {
                    // Process each drive
                    (DriveModel updatedDrive, existingDrivesDict) = 
                        await ProcessDriveAsync(drive, machine.Id, existingDrivesDict, connection, transaction);

                    updatedDrivesList.Add(updatedDrive);

                    // Insert new drive history
                    int historyDriveId = await drivesRepository.InsertHistoryAsync(drive, connection, transaction);

                    // Process drive's OS
                    await ProcessOsAsync(drive, historyDriveId, connection, transaction);

                    // Process drive's Apps
                    if (drive.AppList != null)
                    {
                        var existingDrive = existingMachine.Drives.FirstOrDefault(ed => ed.SerialNumber == drive.SerialNumber);
                        await ProcessAppsAsync(drive, existingDrive, historyDriveId, connection, transaction);
                    }
                }

                await DeleteRemainingDrivesAsync(existingDrivesDict, connection, transaction);

                return updatedDrivesList;
            }

            async Task<(DriveModel, Dictionary<string, DriveModel>)> ProcessDriveAsync(DriveModel drive, int machineId, Dictionary<string, DriveModel> existingDrivesDict, SqlConnection connection, SqlTransaction transaction)
            {
                drive.MachineId = machine.Id;
                DriveModel updatedDrive;

                // If one of the drives in the database has this drive's serial number...
                if (existingDrivesDict.ContainsKey(drive.SerialNumber))
                {
                    // ...update it and remove it from the dictionary (so we can tell it has been processed).
                    updatedDrive = await drivesRepository.UpdateAsync(drive, connection, transaction);
                    existingDrivesDict.Remove(drive.SerialNumber);
                }
                else
                {
                    // Otherwise create a new one.
                    ConsoleUtils.LogDriveCreation(drive.Name, drive.SerialNumber);
                    updatedDrive = await drivesRepository.InsertAsync(drive, connection, transaction);
                }

                drive.Id = updatedDrive.Id;
                return (updatedDrive, existingDrivesDict);
            }

            async Task ProcessOsAsync(DriveModel drive, int historyDriveId, SqlConnection connection, SqlTransaction transaction)
            {
                if (drive.IsSystemDrive && drive.Os != null)
                {
                    drive.Os.DriveId = drive.Id;
                    OsModel updatedOs = await osRepository.UpdateAsync(drive.Os, connection, transaction);
                    drive.Os.Id = updatedOs.Id;

                    // Create OS history
                    await osRepository.InsertHistoryAsync(drive.Os, connection, transaction, historyDriveId);
                }
            }

            async Task ProcessAppsAsync(DriveModel drive, DriveModel? existingDrive, int historyDriveId, SqlConnection connection, SqlTransaction transaction)
            {
                // If the drive already exists we can do comparisons
                if (existingDrive != null)
                {
                    var existingAppsDict = existingDrive?.AppList?.ToDictionary(app => app.Id) ?? new Dictionary<int, ApplicationModel>();

                    foreach (ApplicationModel app in drive.AppList)
                    {
                        app.DriveId = drive.Id;

                        // Check if the app already has a relation with this drive
                        // If not, create instead of update
                        if (existingAppsDict != null && existingAppsDict.ContainsKey(app.Id))
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
                    foreach (ApplicationModel app in drive.AppList)
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
            await using SqlConnection connection = CreateConnection();
            return await machinesRepository.GetAllAsync(connection);
        }
    }
}
