//using Microsoft.AspNetCore.Mvc;
//using SystemInfoApi.Models;

//namespace SystemInfoApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CustomersController : ControllerBase
//    {
//        private readonly SystemInfoContext _context;

//        public CustomersController(SystemInfoContext context)
//        {
//            _context = context;
//        }

//        // POST: api/CustomerModels
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPost]
//        public async Task<ActionResult<CustomerModel>> PostCustomerModel(CustomerModel customerModel)
//        {
//            _context.Customers.Add(customerModel);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetCustomerModel", new { id = customerModel.Id }, customerModel);
//        }

//        // GET: api/CustomerModels
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<CustomerModel>>> GetCustomerModel()
//        {
//            return await _context.Customers.ToListAsync();
//        }

//        // GET: api/CustomerModels/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<CustomerModel>> GetCustomerModel(int? id)
//        {
//            var customerModel = await _context.Customers
//                                      .Include(c => c.Machines)
//                                      .ThenInclude(m => m.Drives)
//                                      .ThenInclude(d => d.Os)
//                                      .FirstOrDefaultAsync(c => c.Id == id);

//            if (customerModel == null) {
//                return NotFound();
//            }

//            return customerModel;
//        }

//        // PUT: api/CustomerModels/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutCustomerModel(int? id, CustomerModel customerModel)
//        {
//            if (id != customerModel.Id)
//            {
//                return BadRequest();
//            }

//            _context.Entry(customerModel).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!CustomerModelExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }


//        // DELETE: api/CustomerModels/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteCustomerModel(int? id)
//        {
//            var customerModel = await _context.Customers.FindAsync(id);
//            if (customerModel == null)
//            {
//                return NotFound();
//            }

//            _context.Customers.Remove(customerModel);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool CustomerModelExists(int? id)
//        {
//            return _context.Customers.Any(e => e.Id == id);
//        }
//    }
//}
