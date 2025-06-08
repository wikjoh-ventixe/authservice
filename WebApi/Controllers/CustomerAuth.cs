using Business.Dtos;
using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerAuth(IAuthService authService, IVerificationService verificationService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IVerificationService _verificationService = verificationService;


    [HttpPost]
    public async Task<IActionResult> RegisterCustomer(CustomerRegistrationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterCustomer(request);
        return result.Succeeded ? Created((string?)null, result.Data) : StatusCode(result.StatusCode, result.ErrorMessage);
    }
}
