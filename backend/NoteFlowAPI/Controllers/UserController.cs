namespace NoteFlowAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly() => Ok(new { Message = "Welcome, Admin!" });

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(new { Email = email });
    }
}

