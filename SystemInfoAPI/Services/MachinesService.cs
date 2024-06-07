using SystemInfoApi.Models;

namespace SystemInfoApi.Services
{
    public interface IMachineService
    {
        Task<MachineModel> CreateMachineAsync(MachineModel machine);
        Task<MachineModel> GetMachineByIdAsync(int machineId);
    }

    public class MachinesService : IMachineService
    {
        private readonly IMachinesRepository _machinesRepository;
        private readonly IDrivesRepository _drivesRepository;
        private readonly IOsRepository _osRepository;

        public MachinesService()


    }
}
