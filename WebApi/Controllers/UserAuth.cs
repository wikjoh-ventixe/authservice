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
}
