using Microsoft.AspNetCore.Mvc;
using RetailBanking.Constants;
using RetailBanking.Interfaces;
using RetailBanking.Models;
using RetailBanking.Services;

namespace RetailBanking.Controllers
{
    [Route("RetailBanking-api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly NotificationPublisher _notificationPublisher;
        public LoanController(ILoanService loanService, NotificationPublisher notificationPublisher)
        {
            _loanService = loanService;
            _notificationPublisher = notificationPublisher;
        }

        [HttpGet("schemes")]
        public async Task<IActionResult> GetLoanSchemes()
        {
            var schemes = await _loanService.GetLoanSchemesAsync();

            return Ok(schemes);
        }
        
        [HttpPost("calculateemi")]
        public async Task<IActionResult> CalculateEmi(EmiRequest request)
        {
            var emi = await _loanService.CalculateEMIAsync(request.LoanAmount,request.InterestRate,request.TenureMonths);

            return Ok(new { EMI = emi });
        }

        [HttpPost("loanapply")]
        public async Task<IActionResult> ApplyLoan(LoanApplication loanApplication)
        {
            //get kyc details
            LoanResponse loanresponse = new LoanResponse();
            var custdetails = await _loanService.GetCustomerDetails(loanApplication.CustomerId);
            if (custdetails.KycStatus != KycStatus.Approved.ToString())
                loanresponse.ErrorMessage = "Loan application is allowed only for KYC approved customers.";
            if (string.IsNullOrEmpty(loanresponse.ErrorMessage))
            {
                loanresponse = await _loanService.SubmitLoanApplicationAsync(loanApplication);
                CreatedAtAction(nameof(GetApplicationStatus), new { id = loanresponse.LoanAppicationId }, loanresponse);
            }

            //notification sent to service bus
            await _notificationPublisher.PublishAsync(new LoanResponse
            {
                LoanAppicationId = loanresponse.LoanAppicationId,
                CustomerName = custdetails.FullName!,
                EmailAddress = custdetails.EmailAddress!,
                LoanStatus = loanresponse.LoanStatus,
                LoanAmount = loanApplication.LoanAmount,
                InterestRate = loanApplication.InterestRate,
                Remarks = loanresponse.ErrorMessage
            });

            return Ok(loanresponse);
        }

        [HttpGet("status/{id}")]
        public async Task<IActionResult> GetApplicationStatus(int id)
        {
            var application =await _loanService.GetApplicationByIdAsync(id);

            if (application == null)
            {
                return NotFound("Application not found.");
            }

            return Ok(new
            {
                ApplicationId = application.LoanApplicationId,
                Status = application.ApplicationStatus,
                AppliedDate = application.ApplicationDate
            });
        }
    }
}
