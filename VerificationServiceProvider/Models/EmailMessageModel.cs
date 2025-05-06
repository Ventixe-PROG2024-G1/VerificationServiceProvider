namespace VerificationServiceProvider.Models
{
    public class EmailMessageModel
    {
        public List<string> Recipients { get; set; } = null!;

        public string? Subject { get; set; }

        public string? PlainText { get; set; }

        public string? Html { get; set; }
    }
}