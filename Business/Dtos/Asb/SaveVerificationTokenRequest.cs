using System.ComponentModel.DataAnnotations;

namespace Business.Dtos.Asb;

public class SaveVerificationTokenRequest
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Token { get; set; } = null!;

    public TimeSpan ValidFor { get; set; }
}