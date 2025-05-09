namespace VerificationServiceProvider.Models
{
    public class VerificationResponse
    {
        public bool Succeeded { get; set; }

        public string? Message { get; set; }

        public string? Error { get; set; }
    }
}