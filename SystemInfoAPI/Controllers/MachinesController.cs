using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Models;
using SystemInfoApi.Services;

namespace SystemInfoApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachinesController(MachinesService machinesService) : ControllerBase
    {
        // POST: api/<Machines>/Create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<MachineModel>> Create([FromBody] MachineModel machine)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Failed to validate model.");
                return BadRequest(ModelState);
            }

            try
            {
                MachineModel newMachine = await machinesService.CreateMachineAsync(machine);

                if (newMachine == null)
                {
                    return StatusCode(500, "An error occured creating the new machine. Machine is null.");
                }
                else
                {
                    CreatedAtActionResult response = CreatedAtAction(nameof(GetById), new { machineId = newMachine.Id }, newMachine);
                    RouteValueDictionary? routeValues = response.RouteValues;
                    string? location = Url.Action(nameof(GetById), new { machineId = routeValues["machineId"] });

                    Console.WriteLine(
                        "\r\n" +
                        "A new machine has been created in the database. \r\n" +
                        $"Time: {DateTime.Now} \r\n" +
                        $"Customer ID: {newMachine.CustomerId} \r\n" +
                        $"Machine ID: {newMachine.Id} \r\n" +
                        $"Machine name: {newMachine.Name} \r\n" +
                        $"Drives amount: {newMachine.Drives.Count} \r\n" +
                        $"Location: {location}"
                    );
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error.");
            }
        }

        // GET: api/<Machines>/GetAll
        [HttpGet]
        public async Task<ActionResult<List<MachineModel>>> GetAll()
        {
            List<MachineModel> machinesList = await machinesService.GetAllAsync();

            if (machinesList.Count > 0)
            {
                return Ok(machinesList);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: api/<Machines>/GetById/{id}
        [HttpGet("{machineId:int:min(0)}")]
        public async Task<ActionResult<MachineModel>> GetById(int machineId)
        {
            MachineModel machine = await machinesService.GetByIdAsync(machineId);

            if (machine.Id != 0)
            {
                return Ok(machine);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{machineName}")]
        public string GetByName(string machineName)
        {
            return $"Name is : {machineName}";
        }

        [HttpGet("{customerId:int:min(0)}")]
        public string GetByCustomerId(int customerId)
        {
            return $"Customer Id is : {customerId}";
        }
    }
}
