using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SystemInfoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ValuesController(IConfiguration configuration) {
            _configuration = configuration;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<KeyValuePair<string, string?>> Get() {
            return _configuration.GetSection("ConnectionStrings").AsEnumerable();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id) {
            return $"id is : {id}";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value) {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
