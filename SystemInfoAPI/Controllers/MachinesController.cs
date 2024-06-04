using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Models;

namespace SystemInfoApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {

        // POST: <MachinesController>/Create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public void Create(MachineModel machineModel) {
        }

        // GET: <MachinesController>/Get
        [HttpGet(Name = "GetMachineTestName")]
        public void GetAll()
        {
            Console.WriteLine("Machines/Get route called");
        }

        [Route("{id}")]
        [HttpGet]
        public string GetById(int id) {
            return $"Id is : {id}";
        }

        [Route("{clientId}")]
        [HttpGet]
        public string GetByClientId(int clientId) {
            return $"Id is : {clientId}";
        }
    }
}
