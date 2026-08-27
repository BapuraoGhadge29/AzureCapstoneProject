using Microsoft.EntityFrameworkCore;
using RetailBanking.Constants;
using RetailBanking.Models;
using RetailBanking.Repository;
using RetailBanking.Repository.Interfaces;
using System.Net;
namespace RetailBanking.Services
{
    public class CustomerService : IcustomerService
    {
        APIResponse _response = new APIResponse();
        private readonly RetailBankingDbContext _context;
        public CustomerService(RetailBankingDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> CreateCustomer(Customer data)
        {
            data.KycStatus = KycStatus.Pending!.ToString();
            await _context.Customers.AddAsync(data);
            await _context.SaveChangesAsync();
            _response.Result = data.Id.ToString();
            _response.Message = "Customer Created";
            _response.ResponseCode = (int)HttpStatusCode.OK;
            return await Task.FromResult(_response);
        }
        public async Task<APIResponse> DeleteCustomer(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            _context.Customers.Remove(customer!);
            _context.SaveChanges();
            _response.Result = customer!.Id.ToString();
            _response.Message = "Customer Deleted";
            _response.ResponseCode = (int)HttpStatusCode.OK;
            return (_response);
        }
        public async Task<List<Customer>> GetAllCustomers()
        {
            var allCustomers = await _context.Customers.ToListAsync();
            return allCustomers;
        }
        public async Task<Customer> GetcustomerById(int? customerId)
        {
            var _response = await _context.Customers
              .FirstOrDefaultAsync(m => m.Id == customerId);
            return _response;
        }
        public async Task<APIResponse> UpdateCustomer(Customer data, int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            customer.FullName = data.FullName;
            customer.Address = data.Address;
            customer.Dob = data.Dob;
            _context.Customers.Update(customer);
            _context.SaveChanges();
            _response.Message = "Customer Updated";
            _response.ResponseCode = (int)HttpStatusCode.OK;
            return (_response);
        }
        public async Task DocumentSaveToDb(DocumentDetails documentDetails)
        {
            _context.DocumentDetails.Add(documentDetails);
            await _context.SaveChangesAsync();
        }       
    }
}
