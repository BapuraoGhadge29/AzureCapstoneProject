using RetailBanking.Models;

namespace RetailBanking.Repository.Interfaces
{
    public interface IcustomerService
    {
        Task<APIResponse> CreateCustomer(Customer customer);
        Task<APIResponse> UpdateCustomer(Customer customer,int customerId);
        Task<APIResponse> DeleteCustomer(int customerId);
        Task<List<Customer>> GetAllCustomers();
        Task<Customer> GetcustomerById(int ?customerId);
    }
}
