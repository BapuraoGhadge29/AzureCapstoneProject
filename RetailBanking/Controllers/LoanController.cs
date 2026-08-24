using Microsoft.AspNetCore.Mvc;
using RetailBanking.Constants;
using RetailBanking.Interfaces;
using RetailBanking.Models;

namespace RetailBanking.Controllers
{
    [Route("RetailBanking-api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
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
            //if (loanApplication.Customer.KycStatus != KycStatus.Approved)
            //{
            //    return BadRequest(
            //        "Loan application is allowed only for KYC approved customers.");
            //}
            var result = await _loanService.SubmitLoanApplicationAsync(loanApplication);

            return CreatedAtAction(nameof(GetApplicationStatus),new { id = result.LoanApplicationId },result);
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
