using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailBanking.Models;
using RetailBanking.Repository.Interfaces;
using System.Security.Claims;

namespace RetailBanking.Controllers
{
    [Route("RetailBanking-api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly IcustomerService _customerservice;
        public CustomersController(IcustomerService customerService)
        {
            _customerservice = customerService;
        }
       
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var allCustomers = await _customerservice.GetAllCustomers();
            return Ok(allCustomers);
        }
       
        [HttpGet("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(int? id)
        {           
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _customerservice.GetcustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
        
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {            
            if (ModelState.IsValid)
            {
                var response = await _customerservice.CreateCustomer(customer);               
                return Ok(response);
            }
            return BadRequest();
        }
       
        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(Customer customer, int id)
        {           
            var response = await _customerservice.UpdateCustomer(customer, id);
            return Ok(response);
        }
       
        [HttpDelete("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {           
            var response = await _customerservice.DeleteCustomer(id);
            return Ok(response);
        }
    }
}

