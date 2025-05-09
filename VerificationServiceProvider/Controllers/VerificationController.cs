//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using VerificationServiceProvider.Models;
//using VerificationServiceProvider.Services;

//namespace VerificationServiceProvider.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class VerificationController(VerificationService verificationService) : ControllerBase
//    {
//        private readonly VerificationService _verificationService = verificationService;

//        [HttpPost("send")]
//        public async Task<IActionResult> Send(SendVerificationCodeRequest request)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(new { Error = "Recipient email address is required" });
//            }

//            var result = await _verificationService.SendVerificationCodeAsync(request);
//            return result.Succeeded ? Ok(result) : StatusCode(500, result);
//        }

//        [HttpPost("verify")]
//        public IActionResult Verify(VerifyVerificationCodeRequest request)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(new { Error = "Invalid or expired verification code" });
//            }

//            var result = _verificationService.VerifyVerificationCode(request);
//            return result.Succeeded ? Ok(result) : StatusCode(500, result);
//        }
//    }
//}