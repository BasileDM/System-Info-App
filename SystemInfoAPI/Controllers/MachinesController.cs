using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;
using SystemInfoApi.Services;

namespace SystemInfoApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachinesController(MachinesService machinesService) : ControllerBase
    {
        // POST: api/<Machines>/Create
        [Authorize]
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
                Console.WriteLine();
                Console.WriteLine("A new machine has been created in the database.");
                Console.WriteLine($"Time: {DateTime.Now.ToLocalTime}");
                Console.WriteLine($"Customer ID: {newMachine.CustomerId}");
                Console.WriteLine($"Machine ID: {newMachine.Id}");
                Console.WriteLine($"Machine name: {newMachine.Name}");
                Console.WriteLine($"Drives amount: {newMachine.Drives.Count}");
                Console.WriteLine($"Location: {location}");

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
        [Authorize]
        [HttpPut("{machineId:int:min(0)}")]
        [Consumes("application/json")]
        public async Task<ActionResult<MachineModel>> Update(int machineId, [FromBody] MachineModel machine)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Failed to validate model: " + ModelState);
                return BadRequest("Invalid request, check API logs for more information.");
            }

            if (machineId != machine.Id)
            {
                Console.WriteLine($"Machine Id mismatch. Route was Update/{machineId}, but machine id was {machine.Id}");
                return BadRequest("Machine Id error.");
            }

            try
            {
                MachineModel updatedMachine = await machinesService.UpdateFullMachineAsync(machine);

                if (updatedMachine == null)
                {
                    return NotFound($"Machine with ID {machineId} was not found.");
                }
                return CreatedAtRoute(nameof(GetById), new { machineId = updatedMachine.Id }, updatedMachine);
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
        [Authorize]
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
        [Authorize]
        [HttpGet("{machineId:int:min(0)}", Name = nameof(GetById))]
        public async Task<ActionResult<MachineModel>> GetById(int machineId)
        {
            Console.WriteLine($"Issuing request to get a machine by ID. Id: {machineId}");
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
