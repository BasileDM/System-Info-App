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

        // POST: <Machines>/Create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public void Create(MachineModel machineModel) {
        }

        // GET: <Machines>/GetAll
        [HttpGet]
        public ActionResult<List<MachineModel>> GetAll() {

            List<MachineModel> machinesList = _MachinesRepository.GetAll();

            if (machinesList.Count > 0) {
                return Ok(machinesList);

            } else { return NotFound(); }
        }

        // GET: <Machines>/GetById/{id}
        [HttpGet("{machineId:int:min(0)}")]
        public ActionResult<MachineModel> GetById(int machineId) {
            var machine = new MachineModel()
            {
                Id = machineId,
                Name = "Machine1"
            };

            if (machine.Id == 0) {
                return Ok(machine);

            } else { return NotFound(); }
        }

        [HttpGet("{machineName}")]
        public string GetByName(string machineName) {
            return $"Name is : {machineName}";
        }

        [HttpGet("{clientId:int:min(0)}")]
        public string GetByClientId(int clientId) {
            return $"Client Id is : {clientId}";
        }
    }
}
