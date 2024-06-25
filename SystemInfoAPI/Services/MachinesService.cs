using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;
using SystemInfoApi.Repositories;

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

                    if (drive.IsSystemDrive && drive.Os != null)
                    {
                        // Set new driveId on OS and insert
                        drive.Os.DriveId = updatedDrive.Id;
                        OsModel updatedOs = await osRepository.InsertAsync(drive.Os, connection, transaction);
                        updatedDrive.Os = updatedOs;
                    }

                    foreach (ApplicationModel app in drive.AppList)
                    {
                        app.DriveId = updatedDrive.Id;
                        await appRepository.InsertAsync(app, connection, transaction);
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

                // Update machine's drives list
                List<DriveModel> updatedDrivesList = [];
                foreach(DriveModel drive in machine.Drives)
                {
                    // Update drive with machine Id and drive Id
                    drive.MachineId = machine.Id;
                    DriveModel updatedDrive = await drivesRepository.UpdateAsync(drive, connection, transaction);
                    drive.Id = updatedDrive.Id;

                    // Update drive's OS with drive ID and OS Id
                    if (drive.IsSystemDrive && drive.Os != null) 
                    {
                        drive.Os.DriveId = drive.Id;
                        OsModel updatedOs = await osRepository.UpdateAsync(drive.Os, connection, transaction);
                        drive.Os.Id = updatedOs.Id;
                    }

                    // Update each drive's app with drive Id
                    if (drive.AppList != null)
                    {
                        foreach (ApplicationModel app in drive.AppList)
                        {
                            app.DriveId = drive.Id;

                            // Check if the app already has a realation with this drive
                            // If not, create instead of update
                            if (!await appRepository.DoesAppDriveRelationExist(app.Id, app.DriveId, connection, transaction))
                            {
                                Console.WriteLine($"App {app.Name} with id n°{app.Id} is new on drive {app.DriveId}. Inserting a new relation.");
                                await appRepository.InsertAsync(app, connection, transaction);
                            }
                            else
                            {
                                await appRepository.UpdateAsync(app, connection, transaction);
                            }
                        } 
                    }
                    updatedDrivesList.Add(drive);
                }
                machine.Drives = updatedDrivesList;
                Console.WriteLine("Machine updated successfully :\r\n");

                return machine;
            });
        }

        public async Task<MachineModel> GetByIdAsync(int machineId)
        {
            await using SqlConnection connection = CreateConnection();
            return await machinesRepository.GetByIdAsync(machineId, connection);
        }

        public async Task<List<MachineModel>> GetAllAsync()
        {
            await using SqlConnection connection = CreateConnection();
            return await machinesRepository.GetAllAsync(connection);
        }
    }
}
