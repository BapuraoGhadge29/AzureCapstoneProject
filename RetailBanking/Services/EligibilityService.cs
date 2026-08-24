using Microsoft.Extensions.Options;
using RetailBanking.Interfaces;
using RetailBanking.Models;

namespace RetailBanking.Services
{
    public class EligibilityService : IEligibilityService
    {
        private readonly CreditRules _rules;

        public EligibilityService(IOptions<CreditRules> options)
        {
            _rules = options.Value;
        }

        public bool CheckEligibility(LoanApplication application)
        {
            return application.AnnualIncome >= _rules.MinIncome
                && application.CreditScore >= _rules.MinCreditScore;
        }
    }
}
