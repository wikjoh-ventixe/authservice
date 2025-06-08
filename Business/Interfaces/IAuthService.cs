using Business.Dtos;
using Business.Models;

namespace Business.Interfaces;

public interface IAuthService
{
    Task<AuthResult<AuthData>> CustomerLoginAsync(CustomerLoginRequestDto request);
    Task<AuthResult<AuthData>> CustomerVerifyEmail(CustomerVerifyEmailRequestDto request);
    Task<AuthResult<AuthData>> RegisterCustomerAsync(CustomerRegistrationRequestDto request);
}
