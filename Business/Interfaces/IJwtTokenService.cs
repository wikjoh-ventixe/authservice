namespace Business.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string email, string userType, Dictionary<string, string>? additionalClaims = null);
    bool ValidateToken(string token);
}