using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Models;

namespace SystemInfoApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {

        // POST: <Machines>/Create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public void Create(MachineModel machineModel) {
        }

        // GET: <Machines>/GetAll
        [HttpGet]
        public ActionResult<List<MachineModel>> GetAll() {
            var machinesList = new List<MachineModel>()
            {
                new() { Name = "Machine1"},
                new() { Name = "Machine2"}
            };

            if (machinesList.Count > 0) {
                return Ok(machinesList);

            } else { return NotFound(); }
        }

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
