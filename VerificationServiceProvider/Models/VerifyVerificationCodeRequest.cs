using System.ComponentModel.DataAnnotations;

namespace VerificationServiceProvider.Models;

public class VerifyVerificationCodeRequest
{
    [Required]
    public string Email { get; set; } = null!;

    public string Code { get; set; } = null!;
}