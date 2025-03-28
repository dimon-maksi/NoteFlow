namespace NoteFlowAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using NoteFlowAPI.Models;
using NoteFlowAPI.Data; 

/// <summary>
/// Controller that handles user operations for the NoteFlow API.
/// </summary>

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    /// <summary>
    /// Endpoint accessible only to users with Admin role.
    /// </summary>
    /// <returns>A welcome message for admin users.</returns>

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly() => Ok(new { Message = "Welcome, Admin!" });

    /// <summary>
    /// Gets information about the currently authenticated user.
    /// </summary>
    /// <returns>The email of the currently authenticated user.</returns>

    [HttpPost("promote-to-admin")]
    public async Task<IActionResult> PromoteToAdmin([FromBody] string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null) return NotFound();

        user.Role = Role.Admin;
        await _userRepository.UpdateUserAsync(user);
        return Ok();
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(new { Email = email });
    }
}

