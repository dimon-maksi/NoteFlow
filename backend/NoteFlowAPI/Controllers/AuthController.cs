namespace NoteFlowAPI.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteFlowAPI.DTOs;
using NoteFlowAPI.Services;

/// <summary>
/// Controller that handles authentication operations for the NoteFlow API.
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly TokenBlacklistService _tokenBlacklistService;
    private readonly TokenService _tokenService;

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
        if (token is null)
            return Unauthorized();
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

    /// <summary>
    /// Logs out a user by invalidating their current JWT token.
    /// </summary>
    /// <returns>A success message indicating the logout was successful.</returns>

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Extract the token from the Authorization header
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            // Get token expiration time from claims
            var expirationClaim = User.Claims.FirstOrDefault(c => c.Type == "exp");
            var expirationTime =
                expirationClaim != null
                    ? DateTimeOffset
                        .FromUnixTimeSeconds(long.Parse(expirationClaim.Value))
                        .UtcDateTime
                    : DateTime.UtcNow.AddHours(2); // Fallback to 2 hours if no expiration claim found

            // Add token to blacklist until it expires
            _tokenBlacklistService.BlacklistToken(token, expirationTime);
        }

        return Ok(new { Message = "logout successful", Token = string.Empty });
    }
}
