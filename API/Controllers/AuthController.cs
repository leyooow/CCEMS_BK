using Microsoft.AspNetCore.Mvc;
using Application.Models.DTOs.Auth;
using System.Threading.Tasks;
using Application.Contracts.Services;
using static Application.Models.DTOs.Auth.AuthenticationDTO;
using Azure.Core;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var response = await _authService.AuthenticateAsync(request);
        return Ok(response);
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var response = await _authService.LogoutAsync();
        return Ok(response);
    }

}
