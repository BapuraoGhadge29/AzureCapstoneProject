using RetailBanking.Models;

namespace RetailBanking.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanScheme>> GetLoanSchemesAsync();
        Task<LoanApplication> SubmitLoanApplicationAsync(LoanApplication loanApplication);
        Task<LoanApplication?> GetApplicationByIdAsync(int id);
        Task<decimal> CalculateEMIAsync(decimal loanAmount, decimal annualInterestRate, int tenureMonths);
    }
}
