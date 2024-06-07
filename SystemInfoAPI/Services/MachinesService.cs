using SystemInfoApi.Models;
using SystemInfoApi.Repositories;

namespace SystemInfoApi.Services
{
    public class MachinesService(MachinesRepository machinesRepository, DrivesRepository drivesRepository, OsRepository osRepository)
    {
        public async Task<MachineModel> CreateMachineAsync(MachineModel machine)
        {
            // Insert machine and get new ID
            MachineModel newMachine = await machinesRepository.InsertAsync(machine);

            foreach (DriveModel drive in machine.Drives)
            {
                // Set new machineId on drive and insert
                drive.MachineId = newMachine.Id;
                DriveModel newDrive = await drivesRepository.InsertAsync(drive);

                if (drive.IsSystemDrive && drive.Os != null)
                {
                    // Set new driveId on OS and insert
                    drive.Os.DriveId = newDrive.Id;
                    await osRepository.InsertAsync(drive.Os);
                }
            }
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
