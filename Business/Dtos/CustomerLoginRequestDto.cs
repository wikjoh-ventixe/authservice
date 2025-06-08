namespace Business.Dtos;

public class CustomerLoginRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
