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
                MachineModel newMachine = await machinesService.InsertFullMachineAsync(machine);
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
            catch (ArgumentException)
            {
                return BadRequest("Invalid request, check API logs for more information.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal error. An unexepected error has occured, check API logs for more information.");
            }
        }

        // PUT : api/<Machines>/Update/{machineId}
        [HttpPut("{machineId:int:min(0)}")]
        [Consumes("application/json")]
        public async Task<ActionResult<MachineModel>> Update(int machineId, [FromBody] MachineModel machine)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Failed to validate model.");
                return BadRequest(ModelState);
            }

            if (machineId != machine.Id)
            {
                Console.WriteLine($"Machine Id mismatch. Route was Update/{machineId}, but machine id was {machine.Id}");
                return BadRequest("Machine Id mismatch.");
            }

            try
            {
                MachineModel updatedMachine = await machinesService.UpdateFullMachineAsync(machine);

                if (updatedMachine == null)
                {
                    return NotFound($"Machine with ID {machineId} was not found.");
                }

                return Ok(updatedMachine);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Invalid request, check API logs for more information.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal error. An unexepected error has occured, check API logs for more information.");
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
    }
}
