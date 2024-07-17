using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Models;
using SystemInfoApi.Services;
using SystemInfoApi.Utilities;

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
            DateTime startTime = DateTime.Now.ToLocalTime();
            ConsoleUtils.LogMachineCreationRequest(HttpContext.Connection, startTime);
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Failed to validate model: " + ModelState);
                return BadRequest("Invalid request, check API logs for more information.");
            }

            try
            {
                MachineModel newMachine = await machinesService.InsertFullMachineAsync(machine);
                CreatedAtActionResult response = CreatedAtAction(nameof(GetById), new { machineId = newMachine.Id }, newMachine);

                ConsoleUtils.LogMachineCreation(response.RouteValues, newMachine, Url, startTime);
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
            var startTime = DateTime.Now.ToLocalTime();
            ConsoleUtils.LogUpdateRequest(machine.Id, HttpContext.Connection, startTime);

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

                ConsoleUtils.LogMachineUpdate(updatedMachine, startTime);
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
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<MachineModel>>> GetAll()
        {
            Console.WriteLine($"Issuing request to get all machines.");
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
            var startTime = DateTime.Now.ToLocalTime();
            ConsoleUtils.LogGetMachineByIdRequest(machineId, HttpContext.Connection, startTime);
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
