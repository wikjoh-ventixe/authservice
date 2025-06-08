namespace Business.Models;

public class AuthData
{
    public string? Token { get; set; }
    public string? UserType { get; set; }
    public string? UserId { get; set; }
    public bool EmailConfirmed { get; set; }
}
