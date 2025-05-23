using System.ComponentModel.DataAnnotations;

namespace VerificationServiceProvider.Models;

public class VerifyVerificationCodeRequestRest
{
    [Required]
    public string Email { get; set; } = null!;

    public string Code { get; set; } = null!;
}