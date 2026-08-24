using RetailBanking.Models;

namespace RetailBanking.Interfaces
{
    public interface IRiskScoreService
    {
        int CalculateRiskScore(LoanApplication application);
    }
}
