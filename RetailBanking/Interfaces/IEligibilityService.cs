using RetailBanking.Models;

namespace RetailBanking.Interfaces
{
    public interface IEligibilityService
    {
        bool CheckEligibility(LoanApplication application);
    }
}
