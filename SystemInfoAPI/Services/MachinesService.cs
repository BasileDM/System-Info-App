using SystemInfoApi.Models;
using SystemInfoApi.Repositories;

namespace SystemInfoApi.Services
{
    public class MachinesService(MachinesRepository machinesRepository, DrivesRepository drivesRepository, OsRepository osRepository)
    {
        /// <summary>Handles business logic to create a machine in the database, making calls to the proper repositories.</summary>
        /// <param name="machine">The <see cref="MachineModel"/> object without IDs to handle.</param>
        /// <returns>
        ///   <br /> A new <see cref="MachineModel"/> object with the created IDs from the database
        /// </returns>
        public async Task<MachineModel> CreateMachineAsync(MachineModel machine)
        {
            // Insert machine and get new ID
            MachineModel newMachine = await machinesRepository.InsertAsync(machine);
            List<DriveModel> updatedDrives = [];

            foreach (DriveModel drive in machine.Drives)
            {
                // Set new machineId on drive and insert
                drive.MachineId = newMachine.Id;
                DriveModel newDrive = await drivesRepository.InsertAsync(drive);

                if (drive.IsSystemDrive && drive.Os != null)
                {
                    // Set new driveId on OS and insert
                    drive.Os.DriveId = newDrive.Id;
                    OsModel newOs = await osRepository.InsertAsync(drive.Os);

                    newDrive.Os = newOs;
                }
                updatedDrives.Add(newDrive);
            }
            newMachine.Drives = updatedDrives;
            return newMachine;
        }

        public async Task<MachineModel> GetByIdAsync(int machineId)
        {
            return await machinesRepository.GetByIdAsync(machineId);
        }

        public async Task<List<MachineModel>> GetAllAsync()
        {
            return await machinesRepository.GetAllAsync();
        }
    }
}
