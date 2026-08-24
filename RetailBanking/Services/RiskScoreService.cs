using RetailBanking.Interfaces;
using RetailBanking.Models;

namespace RetailBanking.Services
{
    public class RiskScoreService : IRiskScoreService
    {
        public int CalculateRiskScore(LoanApplication application)
        {
            int score = 0;

            score += application.CreditScore / 10;

            if (application.AnnualIncome >= 500000)
                score += 15;

            if (application.EmploymentYears >= 5)
                score += 10;

            return Math.Min(score, 100);
        }
    }
}
