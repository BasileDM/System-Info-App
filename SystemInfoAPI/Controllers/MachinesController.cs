using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Models;
using SystemInfoApi.Repositories;

namespace SystemInfoApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachinesController(MachinesRepository machinesRepository) : ControllerBase
    {
        private readonly MachinesRepository _MachinesRepository = machinesRepository;

        // POST: api/<Machines>/Create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<MachineModel>> Create(MachineModel machine) {
            MachineModel newMachine = await _MachinesRepository.PostAsync(machine);

            if (newMachine == null) {
                return UnprocessableEntity();
            } else {
                return Ok(newMachine);
            }

        }

        // GET: api/<Machines>/GetAll
        [HttpGet]
        public async Task<ActionResult<List<MachineModel>>> GetAll() {
            List<MachineModel> machinesList = await _MachinesRepository.GetAllAsync();

            if (machinesList.Count > 0) {
                return Ok(machinesList);

            } else { return NotFound(); }
        }

        // GET: api/<Machines>/GetById/{id}
        [HttpGet("{machineId:int:min(0)}")]
        public async Task<ActionResult<MachineModel>> GetById(int machineId) {
            MachineModel machine = await _MachinesRepository.GetByIdAsync(machineId);

            if (machine.Id != null) {
                return Ok(machine);

            } else { return NotFound(); }
        }

        [HttpGet("{machineName}")]
        public string GetByName(string machineName) {
            return $"Name is : {machineName}";
        }

        [HttpGet("{customerId:int:min(0)}")]
        public string GetByCustomerId(int customerId) {
            return $"Customer Id is : {customerId}";
        }
    }
}
