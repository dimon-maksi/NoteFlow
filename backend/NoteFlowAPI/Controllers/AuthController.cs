namespace NoteFlowAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using NoteFlowAPI.Services;
using NoteFlowAPI.DTOs;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
    {
        var token = await _authService.AuthenticateAsync(loginDto);
        if (token is null) return Unauthorized();
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
    {
        await _authService.RegisterAsync(registerDto);
        return Ok(new { Message = "User registered successfully" });
    }
}

