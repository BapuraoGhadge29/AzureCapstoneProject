using Microsoft.AspNetCore.Mvc;
using RetailBanking.Interfaces;

namespace RetailBanking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApproverController : ControllerBase
    {
        private readonly IKycService _kycService;

        public ApproverController(IKycService kycService)
        {
            _kycService = kycService;
        }

        [HttpPut("approvekyc/{customerId}")]
        public async Task<IActionResult> ApproveCustomer(int customerId)
        {
            var result = await _kycService.ApproveKycAsync(customerId);

            if (!result)
            {
                return NotFound("Customer not found.");
            }

            return Ok("Customer KYC approved.");
        }

        [HttpPut("rejectkyc/{customerId}")]
        public async Task<IActionResult> RejectCustomer(int customerId)
        {
            var result = await _kycService.RejectKycAsync(customerId);

            if (!result)
            {
                return NotFound("Customer not found.");
            }

            return Ok("Customer KYC rejected.");
        }
    }
}
