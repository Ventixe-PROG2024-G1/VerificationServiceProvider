using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using VerificationServiceProvider.Models;

namespace VerificationServiceProvider.Services
{
    public interface IVerificationService
    {
        void SaveVerificationCode(SaveVerificationCodeRequest request);

        Task<VerificationResponse> SendVerificationCode(SendVerificationCodeRequest request, ServerCallContext context);

        Task<VerificationResponse> VerifyVerificationCode(VerifyVerificationCodeRequest request, ServerCallContext context);
    }

    public class VerificationService(IConfiguration configuration, IMemoryCache cache, EmailContract.EmailContractClient emailServiceClient) : VerificationContract.VerificationContractBase, IVerificationService
    {
        private readonly IConfiguration _configuration = configuration;

        private readonly IMemoryCache _cache = cache;
        private static readonly Random _random = new();
        private readonly EmailContract.EmailContractClient _emailServiceClient = emailServiceClient;

        public override async Task<VerificationResponse> SendVerificationCode(SendVerificationCodeRequest request, ServerCallContext context)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                {
                    return new VerificationResponse { Succeeded = false, Error = "Recipient email address is required" };
                }

                var verificationCode = _random.Next(100000, 999999).ToString();
                var subject = $"Your code is {verificationCode}";
                // PlainText för Epost-klienter som inte klarar av html-formaterade epost-meddelanden
                var plainTextContent = $@"
                Verify Your Email Address

                Hello. To complete your cerification, pleaste enter the following code:

                {verificationCode}

                ";

                var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; background-color: #fff; padding: 20px;'>
                    <div style='background-color: #f9f9f9; border: 1px solid #F26CF9; border-radius: 8px; padding: 20px; max-width: 600px; margin: auto;'>
                        <h1 style='color: #F26CF9;'>Verify Your Email Address</h1>
                        <p>Hello,</p>
                        <p>To complete your registration at Ventixe, please enter the following code:</p>
                        <p style='font-weight: bold; color: #F26CF9; background-color: #fff; padding: 10px; border-radius: 5px; display: inline-block;'>{verificationCode}</p>
                        <p>Thank you!</p>
                    </div>
                </body>
                </html>";

                var emailRequest = new EmailMessageRequest
                {
                    Recipients = { request.Email },
                    Subject = subject,
                    PlainText = plainTextContent,
                    Html = htmlContent
                };

                var response = await _emailServiceClient.SendEmailAsync(emailRequest);

                if (response.Succeeded)
                {
                    SaveVerificationCode(new SaveVerificationCodeRequest { Email = request.Email, Code = verificationCode, ValidFor = TimeSpan.FromMinutes(5) });
                }

                return response.Succeeded
                    ? new VerificationResponse { Succeeded = true }
                    : new VerificationResponse { Succeeded = false, Message = "Unable to send verification code" };
            }
            catch (Exception ex)
            {
                return new VerificationResponse { Succeeded = false, Error = ex.Message };
            }
        }

        public void SaveVerificationCode(SaveVerificationCodeRequest request)
        {
            _cache.Set(request.Email.ToLowerInvariant(), request.Code, request.ValidFor);
        }

        public override async Task<VerificationResponse> VerifyVerificationCode(VerifyVerificationCodeRequest request, ServerCallContext context)
        {
            var key = request.Email.ToLowerInvariant();

            if (_cache.TryGetValue(key, out string? storedCode))
            {
                // Om den lagrade koden är samma sak som Code är isValid true
                if (storedCode == request.Code)
                {
                    _cache.Remove(key);
                    return new VerificationResponse { Succeeded = true, Message = "Verification successful." };
                }
            }

            return new VerificationResponse { Succeeded = false, Error = "Invalid or expired verification code." };
        }
    }
}