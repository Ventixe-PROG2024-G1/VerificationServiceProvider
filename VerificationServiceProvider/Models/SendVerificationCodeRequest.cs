using System.ComponentModel.DataAnnotations;

namespace VerificationServiceProvider.Models
{
    public class SendVerificationCodeRequest
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}