using Business.Dtos;
using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAuth(IAuthService authService, IVerificationService verificationService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IVerificationService _verificationService = verificationService;

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.UserLoginAsync(request);
        return result.Succeeded ? Ok(result.Data) : StatusCode(result.StatusCode, result.ErrorMessage);
    }
}
