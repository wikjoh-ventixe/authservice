using Business.Dtos;
using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerAuth(IAuthService authService, IVerificationService verificationService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IVerificationService _verificationService = verificationService;


    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer(CustomerRegistrationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterCustomerAsync(request);
        return result.Succeeded ? Created((string?)null, result.Data) : StatusCode(result.StatusCode, result.ErrorMessage);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginCustomer(LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.CustomerLoginAsync(request);
        return result.Succeeded ? Ok(result.Data) : StatusCode(result.StatusCode, result.ErrorMessage);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
    {
        if (email == null || token == null)
            return BadRequest("Parameters cannot be null.");

        var decodedToken = WebUtility.UrlDecode(token);

        var result = await _authService.CustomerVerifyEmailAsync(new CustomerVerifyEmailRequestDto
        {
            Email = email,
            Token = decodedToken
        });

        return result.Succeeded ? Ok() : StatusCode(result.StatusCode, result.ErrorMessage);
    }
}
