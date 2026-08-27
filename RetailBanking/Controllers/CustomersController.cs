using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly EventGridService _gridService;
        public CustomersController(IcustomerService customerService, IConfiguration configuration, EventGridService gridService)
        {
            _customerservice = customerService;
            _configuration = configuration;
            _gridService = gridService;
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
             var documents = new List<(string DocumentType, IFormFile? File)>
                {
                    ("PAN", customer.PanCard),
                    ("AADHAR", customer.AadharCard),
                    ("INCOMEPROOF", customer.IncomeProof)
                };
                var response = await _customerservice.CreateCustomer(customer);
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

                //await _gridService.PublishCustomerCreated(customer.Id,customer.FullName!);

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
    }
}

