using System;
using CustomerManagement.Base.Models;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.Threading.Tasks;
using CustomerManagement.DataService.Services;

namespace CustomerManagement.Api.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer([FromBody] Customer customer)
        {
            Logger.Debug("CreateCustomer");
            var savedCustomer = await _customerService.CreateCustomer(customer);
            return CreatedAtAction(nameof(GetByIdAsync), new {id = savedCustomer.Id}, savedCustomer);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetByIdAsync(string id)
        {
            var customer = await _customerService.GetById(Guid.Parse(id));
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}