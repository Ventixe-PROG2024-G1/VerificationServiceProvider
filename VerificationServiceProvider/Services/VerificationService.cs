using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Caching.Memory;
using System.Drawing;
using VerificationServiceProvider.Models;
using static System.Net.Mime.MediaTypeNames;

namespace VerificationServiceProvider.Services
{
    public interface IVerificationService
    {
        Task<VerificationServiceResult> SendVerificationCodeAsync(SendVerificationCodeRequest request);

        void SaveVerificationCodeAsync(SaveVerificationCodeRequest request);

        VerificationServiceResult VerifyVerificationCode(VerifyVerificationCodeRequest request);
    }

    public class VerificationService(IConfiguration configuration, EmailClient emailClient, IMemoryCache cache) : IVerificationService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly EmailClient _emailClient = emailClient;
        private readonly IMemoryCache _cache = cache;
        private static readonly Random _random = new();

        public async Task<VerificationServiceResult> SendVerificationCodeAsync(SendVerificationCodeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                {
                    return new VerificationServiceResult { Succeeded = false, Error = "Recipient email address is required" };
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

                var emailMessage = new EmailMessage(
                    senderAddress: _configuration["ACS:SenderAddress"],
                    recipients: new EmailRecipients([new(request.Email)]),
                    content: new EmailContent(subject)
                    {
                        PlainText = plainTextContent,
                        Html = htmlContent
                    });

                var emailSendOperation = await _emailClient.SendAsync(WaitUntil.Started, emailMessage);

                SaveVerificationCodeAsync(new SaveVerificationCodeRequest { Email = request.Email, Code = verificationCode, ValidFor = TimeSpan.FromMinutes(5) });

                return new VerificationServiceResult { Succeeded = true, Message = " Verification email sent successfully" };
            }
            catch (Exception ex)
            {
                return new VerificationServiceResult { Succeeded = false, Error = ex.Message };
            }
        }

        public void SaveVerificationCodeAsync(SaveVerificationCodeRequest request)
        {
            _cache.Set(request.Email.ToLowerInvariant(), request.Code, request.ValidFor);
        }

        public VerificationServiceResult VerifyVerificationCode(VerifyVerificationCodeRequest request)
        {
            var key = request.Email.ToLowerInvariant();

            if (_cache.TryGetValue(key, out string? storedCode))
            {
                // Om den lagrade koden är samma sak som Code är isValid true
                if (storedCode == request.Code)
                {
                    _cache.Remove(key);
                    return new VerificationServiceResult { Succeeded = true, Message = "Verification successful." };
                }
            }

            return new VerificationServiceResult { Succeeded = false, Error = "Invalid or expired verification code." };
        }
    }
}