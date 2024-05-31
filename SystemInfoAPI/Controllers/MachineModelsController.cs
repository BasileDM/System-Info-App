using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemInfoApi.Data;
using SystemInfoApi.Models;

namespace SystemInfoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineModelsController : ControllerBase
    {
        private readonly SystemInfoContext _context;

        public MachineModelsController(SystemInfoContext context)
        {
            _context = context;
        }

        // POST: api/MachineModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MachineModel>> PostMachineModel(MachineModel machineModel) {
            _context.Machines.Add(machineModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMachineModel", new { id = machineModel.Id }, machineModel);
        }

        // GET: api/MachineModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineModel>>> GetMachines()
        {
            return await _context.Machines.ToListAsync();
        }

        // GET: api/MachineModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineModel>> GetMachineModel(int? id)
        {

            var machineModel = await _context.Machines
                                             .Include(m => m.Drives)
                                             .ThenInclude(d => d.Os)
                                             .FirstOrDefaultAsync(m => m.Id == id);

            if (machineModel == null) {
                return NotFound();
            }

            return machineModel;
        }

        // PUT: api/MachineModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMachineModel(int? id, MachineModel machineModel)
        {
            if (id != machineModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(machineModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachineModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/MachineModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineModel(int? id)
        {
            var machineModel = await _context.Machines.FindAsync(id);
            if (machineModel == null)
            {
                return NotFound();
            }

            _context.Machines.Remove(machineModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MachineModelExists(int? id)
        {
            return _context.Machines.Any(e => e.Id == id);
        }
    }
}
