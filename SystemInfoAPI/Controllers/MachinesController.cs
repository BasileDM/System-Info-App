using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Models;
using SystemInfoApi.Repositories;

namespace SystemInfoApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {
        private readonly IConfiguration _Configuration;
        private readonly MachinesRepository _MachinesRepository;

        public MachinesController(IConfiguration config) {
            _Configuration = config;
            _MachinesRepository = new MachinesRepository(_Configuration);
        }

        // POST: api/<Machines>/Create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public void Create(MachineModel machineModel) {
        }

        // GET: api/<Machines>/GetAll
        [HttpGet]
        public ActionResult<List<MachineModel>> GetAll() {

            List<MachineModel> machinesList = _MachinesRepository.GetAll();

            if (machinesList.Count > 0) {
                return Ok(machinesList);

            } else { return NotFound(); }
        }

        // GET: api/<Machines>/GetById/{id}
        [HttpGet("{machineId:int:min(0)}")]
        public ActionResult<MachineModel> GetById(int machineId) {

            MachineModel machine = _MachinesRepository.GetMachineById(machineId);

            if (machine.Id != null) {
                return Ok(machine);

            } else { return NotFound(); }
        }

        [HttpGet("{machineName}")]
        public string GetByName(string machineName) {
            return $"Name is : {machineName}";
        }

        [HttpGet("{customerId:int:min(0)}")]
        public string GetByCustomerId(int clientId) {
            return $"Customer Id is : {clientId}";
        }
    }
}
