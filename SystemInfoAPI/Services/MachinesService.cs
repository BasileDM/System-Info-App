using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;
using SystemInfoApi.Repositories;

namespace SystemInfoApi.Services
{
    public class MachinesService(MachinesRepository machinesRepository, DrivesRepository drivesRepository,
        OsRepository osRepository, IConfiguration config, IWebHostEnvironment env) : Database(config, env)
    {
        /// <summary>Handles SQL connection and transaction to create a new machine in the database.</summary>
        /// <param name="machine">The <see cref="MachineModel"/> object without IDs to handle.</param>
        /// <returns>
        ///   <br /> A new <see cref="MachineModel"/> object with the created IDs from the database.
        /// </returns>
        public async Task<MachineModel> CreateMachineTransactionAsync(MachineModel machine)
        {
            await using SqlConnection connection = GetConnection();
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            try
            {
                MachineModel newMachine = await InsertFullMachineAsync(machine, connection, transaction);
                await transaction.CommitAsync();
                return newMachine;
            }
            catch (ArgumentException ex)
            {
                await transaction.RollbackAsync();
                throw new ArgumentException("Error finalising the transaction with the database. Rolling back...", ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Error finalising the transaction with the database. Rolling back...", ex);
            }
            finally
            {
                await transaction.DisposeAsync();
                await connection.CloseAsync();
            }
        }

        /// <summary>Handles business logic to create a new full machine in the database.</summary>
        /// <param name="machine">The <see cref="MachineModel"/> object without IDs to handle.</param>
        /// <param name="connection">The <see cref="SqlConnection"/> to use.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> object to give the repositories that will be rolled back in case of error.</param>
        /// <returns>
        ///   <br /> A new <see cref="MachineModel"/> object with the created IDs from the database.
        /// </returns>
        private async Task<MachineModel> InsertFullMachineAsync(MachineModel machine, SqlConnection connection, SqlTransaction transaction)
        {
            try
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
                    updatedDrivesList.Add(updatedDrive);
                }
                updatedMachine.Drives = updatedDrivesList;
                return updatedMachine;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Service error while handling the machine creation logic.", ex);
            }
        }

        public async Task<MachineModel> GetByIdAsync(int machineId)
        {
            return await machinesRepository.GetByIdAsync(machineId, GetConnection());
        }

        public async Task<List<MachineModel>> GetAllAsync()
        {
            return await machinesRepository.GetAllAsync(GetConnection());
        }
    }
}
