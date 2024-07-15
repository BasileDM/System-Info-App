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

                // Process drives
                var existingDrivesDict = machine.Drives.ToDictionary(d => d.SerialNumber); // Serial as alt identifier.
                List<DriveModel> updatedDrivesList = [];

                int driveIndex = 0;
                foreach (DriveModel drive in machine.Drives)
                {
                    drive.MachineId = machine.Id;
                    DriveModel updatedDrive;

                    if (existingDrivesDict.ContainsKey(drive.SerialNumber))
                    {
                        // Update existing drive
                        updatedDrive = await drivesRepository.UpdateAsync(drive, connection, transaction);
                        existingDrivesDict.Remove(drive.SerialNumber);
                    }
                    else
                    {
                        // Create a new drive
                        ConsoleUtils.LogDriveCreation(drive.Name, drive.Id, drive.SerialNumber);
                        updatedDrive = await drivesRepository.InsertAsync(drive, connection, transaction);
                    }

                    drive.Id = updatedDrive.Id;
                    updatedDrivesList.Add(drive);
                    // Update drives history
                    int historyDriveId = await drivesRepository.InsertHistoryAsync(drive, connection, transaction);

                    // Process OS
                    if (drive.IsSystemDrive && drive.Os != null) 
                    {
                        drive.Os.DriveId = drive.Id;
                        OsModel updatedOs = await osRepository.UpdateAsync(drive.Os, connection, transaction);
                        drive.Os.Id = updatedOs.Id;

                        // Create OS history
                        await osRepository.InsertHistoryAsync(drive.Os, connection, transaction, historyDriveId);
                    }

                    // Update each drive's app with drive Id
                    var existingDrive = existingMachine.Drives.ElementAt(driveIndex);
                    var existingAppList = existingDrive.AppList;
                    Dictionary<int, ApplicationModel> existingAppDict = existingAppList.ToDictionary(a => a.Id);
                    if (drive.AppList != null)
                    {
                        foreach (ApplicationModel app in drive.AppList)
                        {
                            app.DriveId = drive.Id;

                            // Check if the app already has a relation with this drive
                            // If not, create instead of update
                            if (existingAppDict != null && existingAppDict.ContainsKey(app.Id))
                            {
                                await appRepository.UpdateAsync(app, connection, transaction);
                                existingAppDict.Remove(app.Id);
                            }
                            else
                            {
                                ConsoleUtils.LogAppCreation(app.Name, app.Id, app.DriveId);
                                await appRepository.InsertAsync(app, connection, transaction);
                            }
                            // Create app history
                            await appRepository.InsertHistoryAsync(app, connection, transaction, historyDriveId);
                        }

                    }
                    // Delete remaining old apps not in the new list
                    foreach (var appToDelete in existingAppDict.Values)
                    {
                        ConsoleUtils.LogAppDeletion(appToDelete.Name, appToDelete.Id, existingDrive.Id);
                        await appRepository.DeleteRelationAsync(
                            appToDelete.Id, existingDrive.Id, connection, transaction);
                    }
                    updatedDrivesList.Add(drive);
                    driveIndex++;
                }

                // Delete remaining old drives not in the new list
                foreach (var driveToDelete in existingDrivesDict.Values)
                {
                    ConsoleUtils.LogDriveDeletion(driveToDelete.Name, driveToDelete.Id, driveToDelete.SerialNumber);
                    await drivesRepository.DeleteAsync(driveToDelete.Id, connection, transaction);
                }

                machine.Drives = updatedDrivesList;

                return machine;
            });
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
