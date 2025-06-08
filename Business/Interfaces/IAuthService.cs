using Business.Dtos;
using Business.Models;

namespace Business.Interfaces;

public interface IAuthService
{
    Task<AuthResult<string?>> RegisterCustomer(CustomerRegistrationRequestDto request);
}
