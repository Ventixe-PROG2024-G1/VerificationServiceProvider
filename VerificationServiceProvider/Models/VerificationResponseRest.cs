namespace VerificationServiceProvider.Models
{
    public class VerificationResponseRest
    {
        public bool Succeeded { get; set; }

        public string? Message { get; set; }

        public string? Error { get; set; }
    }
}