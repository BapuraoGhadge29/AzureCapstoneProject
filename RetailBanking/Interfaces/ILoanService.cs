using RetailBanking.Models;

namespace RetailBanking.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanScheme>> GetLoanSchemesAsync();
        Task<LoanResponse> SubmitLoanApplicationAsync(LoanApplication loanApplication);
        Task<LoanApplication?> GetApplicationByIdAsync(int id);
        Task<List<LoanApplication>> GetApplicationsAsync();
        Task<decimal> CalculateEMIAsync(decimal loanAmount, decimal annualInterestRate, int tenureMonths);
        Task<Customer> GetCustomerDetailsAsync(int custId);
        Task ApproveRejectLoanApplicationAsync(int id, string status);
    }
}
