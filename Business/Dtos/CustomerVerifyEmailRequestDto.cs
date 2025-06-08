namespace Business.Dtos;

public class CustomerVerifyEmailRequestDto
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
