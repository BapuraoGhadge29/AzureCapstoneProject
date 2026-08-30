using Azure;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RetailBanking.Hubs;
using RetailBanking.Models;
using RetailBanking.Repository.Interfaces;
using RetailBanking.Services;

namespace RetailBanking.Controllers
{
    [Route("RetailBanking-api/[controller]")]
    [ApiController]
   // [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly IcustomerService _customerservice;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<CustomerHub> _hubContext;
        private readonly ILogger<CustomersController> _logger;
        public CustomersController(IcustomerService customerService, IConfiguration configuration, IHubContext<CustomerHub> hubContext, ILogger<CustomersController> logger)
        {
            _customerservice = customerService;
            _configuration = configuration;
            _hubContext = hubContext;
            _logger = logger;
        }
       
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            _logger.LogInformation("Get all customer called");
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
                await CreateDocumentUploadObject(customer,response);
                //await _gridService.PublishCustomerCreated(customer.Id,customer.FullName!);
                await _hubContext.Clients.All.SendAsync("CustomerCreated",$"New Customer Created Successfully : {customer.FullName}");
                _logger.LogInformation("Customer created"+ customer.FullName);
                return Ok(response);
            }
            return BadRequest();
        }
       
        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(Customer customer, int id)
        {           
            var response = await _customerservice.UpdateCustomer(customer, id);
            await CreateDocumentUploadObject(customer, response);
            _logger.LogInformation("Customer updated" + customer.FullName);
            return Ok(response);
        }
       
        [HttpDelete("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {           
            var response = await _customerservice.DeleteCustomer(id);
            _logger.LogInformation("Customer delete id:" + id);
            return Ok(response);
        }
        [NonAction]
        public async Task<string> UploadFileAsync(IFormFile file, string customerId)
        {
            string connectionString =_configuration["AzureBlobStorage:ConnectionString"]!;
            string containerName =_configuration["AzureBlobStorage:ContainerName"]!;
            
            //connectionString = Environment.GetEnvironmentVariable("azureblobstorageconnectionstring")!;

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = containerClient.GetBlobClient($"{customerId}/{file.FileName}");
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }
            return blobClient.Uri.ToString();
        }
        [NonAction]
        public async Task CreateDocumentUploadObject(Customer customer, APIResponse response)
        {
            var documents = new List<(string DocumentType, IFormFile? File)>
                {
                    ("PAN", customer.PanCard),
                    ("AADHAR", customer.AadharCard),
                    ("INCOMEPROOF", customer.IncomeProof)
                };
            string? documentUrl = null;
            foreach (var document in documents)
            {
                if (document.DocumentType != null && document.File != null)
                {
                    DocumentDetails documentDetails = new DocumentDetails();
                    documentUrl = await UploadFileAsync(document.File, response.Result);
                    documentDetails.DocumentPath = documentUrl;
                    documentDetails.CustomerId = int.Parse(response.Result);
                    await _customerservice.DocumentSaveToDb(documentDetails);
                }
            }
        }
    }
}

