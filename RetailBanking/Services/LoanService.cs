using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RetailBanking.Constants;
using RetailBanking.Interfaces;
using RetailBanking.Models;
using RetailBanking.Repository;

namespace RetailBanking.Services
{
    public class LoanService : ILoanService
    {
        private readonly RetailBankingDbContext _context;
        private readonly IEligibilityService _eligibilityService;
        private readonly IRiskScoreService _riskScoreService;
        private readonly CreditRules _rules;
        public LoanService(RetailBankingDbContext context, IEligibilityService eligibilityService,IRiskScoreService riskScoreService,IOptions<CreditRules> options)
        {
            _eligibilityService = eligibilityService;
            _riskScoreService = riskScoreService;
            _rules = options.Value;
            _context = context;
        }

        public async Task<IEnumerable<LoanScheme>> GetLoanSchemesAsync()
        {
            return await _context.LoanSchemes.ToListAsync();
        }

        public async Task<LoanResponse> SubmitLoanApplicationAsync(LoanApplication loanApplication)
        {
            LoanResponse loanresponse = new LoanResponse();
            loanApplication.ApplicationDate = DateTime.UtcNow;
            loanApplication.ApplicationStatus = ApplicationStatus.Approved.ToString();
            var assessmentResult = LoanAssess(loanApplication);
            if (!assessmentResult.IsEligible)
            {
                loanresponse.ErrorMessage = $"Loan application rejected: {assessmentResult.Remarks}";
                loanApplication.ApplicationStatus = ApplicationStatus.Rejected.ToString();
            }
            if (assessmentResult.Status == "Rejected")
            {
                loanresponse.ErrorMessage = $"Loan application rejected due to credit score is not up to mark";
                loanApplication.ApplicationStatus = ApplicationStatus.Rejected.ToString();
            }
            if (string.IsNullOrEmpty(loanresponse.ErrorMessage))
            {
                _context.LoanApplications.Add(loanApplication);
                await _context.SaveChangesAsync();                
            }

            loanresponse.LoanAppicationId = loanApplication.LoanApplicationId;
            loanresponse.LoanStatus = loanApplication.ApplicationStatus;
            loanresponse.LoanAmount = loanApplication.LoanAmount;
            loanresponse.InterestRate = loanApplication.InterestRate;
            loanresponse.Remarks = loanresponse.ErrorMessage;

            return loanresponse;
        }

        public async Task<LoanApplication?> GetApplicationByIdAsync(int id)
        {
            return await _context.LoanApplications.FirstOrDefaultAsync(x => x.LoanApplicationId == id);
        }
        public async Task<List<LoanApplication>> GetApplicationsAsync()
        {
            var loanApplications= await _context.LoanApplications.ToListAsync();
            return loanApplications;
        }
        public async Task<decimal> CalculateEMIAsync(decimal loanAmount,decimal annualInterestRate,int tenureMonths)
        {
            double principal = (double)loanAmount;

            double monthlyRate =((double)annualInterestRate / 12) / 100;

            double emi =principal * monthlyRate * Math.Pow(1 + monthlyRate, tenureMonths)/(Math.Pow(1 + monthlyRate, tenureMonths) - 1);
            return await Task.FromResult((decimal)Math.Round(emi, 2));
        }
        public CreditAssessmentResult LoanAssess(LoanApplication application)
        {
            if (!_eligibilityService.CheckEligibility(application))
            {
                return new CreditAssessmentResult
                {
                    IsEligible = false,
                    RiskScore = 0,
                    Status = "Rejected",
                    Remarks = "Eligibility criteria not met"
                };
            }

            int riskScore = _riskScoreService.CalculateRiskScore(application);

            string status;

            if (riskScore >= _rules.AutoApproveRiskScore)
                status = "Approved";
            else if (riskScore >= _rules.ManualReviewRiskScore)
                status = "Manual Review";
            else
                status = "Rejected";

            return new CreditAssessmentResult
            {
                IsEligible = true,
                RiskScore = riskScore,
                Status = status,
                Remarks = $"Application classified as {status}"
            };
        }
        public async Task<Customer> GetCustomerDetailsAsync(int custId)
        {
            return await _context.Customers.Where(x => x.Id == custId).FirstOrDefaultAsync()!;
        }
        public async Task ApproveRejectLoanApplicationAsync(int id, string status)
        {
            var loanapplication = await _context.LoanApplications.Where(x => x.LoanApplicationId == id).FirstOrDefaultAsync();
            loanapplication!.ApplicationStatus=status;
            _context.LoanApplications.Update(loanapplication);
            await _context.SaveChangesAsync();
        }
    }
}