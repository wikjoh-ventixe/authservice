using System.ComponentModel.DataAnnotations;

namespace Business.Dtos.Asb;

public class VerifyVerificationTokenRequest
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Token { get; set; } = null!;
}