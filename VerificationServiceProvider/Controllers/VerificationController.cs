using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VerificationServiceProvider.Models;
using VerificationServiceProvider.Services;

namespace VerificationServiceProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController(VerificationService verificationService) : ControllerBase
    {
        private readonly VerificationService _verificationService = verificationService;

        [HttpPost("send-verification-code")]
        public async Task<VerificationResponseRest> Send([FromBody] SendVerificationCodeRequestRest request)
        {
            if (!ModelState.IsValid)
            {
                return new VerificationResponseRest { Error = "Recipient email address is required" };
            }

            var result = await _verificationService.SendVerificationCode(request);
            return result;
        }

        [HttpPost("verify-verification-code")]
        public async Task<VerificationResponseRest> Verify(VerifyVerificationCodeRequestRest request)
        {
            if (!ModelState.IsValid)
            {
                return new VerificationResponseRest { Error = "Invalid or expired verification code" };
            }

            var result = _verificationService.VerifyVerificationCode(request);
            return result;
        }
    }
}