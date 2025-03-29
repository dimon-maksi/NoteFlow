namespace NoteFlowAPI.Controllers;

using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteFlowAPI.Data;
using NoteFlowAPI.Models;

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
    /// Promotes a user to Admin role.
    /// </summary>
    /// <param name="email">The email of the user to promote.</param>
    /// <returns>OK if successful; NotFound if user doesn't exist.</returns>

    [HttpPost("promote-to-admin")]
    public async Task<IActionResult> PromoteToAdmin([FromBody] string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
            return NotFound();

        user.Role = Role.Admin;
        await _userRepository.UpdateUserAsync(user);
        return Ok();
    }

    /// <summary>
    /// Gets information about the currently authenticated user.
    /// </summary>
    /// <returns>The email of the currently authenticated user.</returns>

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(new { Email = email });
    }
}
