using System.ComponentModel.DataAnnotations;

namespace VerificationServiceProvider.Models
{
    public class SendVerificationCodeRequestRest
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}