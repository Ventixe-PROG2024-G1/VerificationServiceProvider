using System.ComponentModel.DataAnnotations;

namespace VerificationServiceProvider.Models
{
    public class SaveVerificationCodeRequest
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Code { get; set; } = null!;

        // Specificerar hur länge koden ska vara giltlig
        public TimeSpan ValidFor { get; set; }
    }
}