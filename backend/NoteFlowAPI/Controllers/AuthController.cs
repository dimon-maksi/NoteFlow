namespace NoteFlowAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using NoteFlowAPI.Services;
using NoteFlowAPI.DTOs;
using System.Threading.Tasks;

/// <summary>
/// Controller that handles authentication operations for the NoteFlow API.
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the AuthController.
    /// </summary>
    /// <param name="authService">The service that handles authentication logic.</param>

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A JWT token for the authenticated user or Unauthorized response.</returns>

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
    {
        var token = await _authService.AuthenticateAsync(loginDto);
        if (token is null) return Unauthorized();
        return Ok(new { Token = token });
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="registerDto">The registration information.</param>
    /// <returns>A confirmation message upon successful registration.</returns>

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
    {
        var token = await _authService.RegisterAsync(registerDto);
        return Ok(new { Token = token });
    }
}

