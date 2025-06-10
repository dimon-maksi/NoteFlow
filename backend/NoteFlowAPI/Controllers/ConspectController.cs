using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteFlowAPI.DTO.Conspect;
using NoteFlowAPI.Helpers.QueryParam;
using NoteFlowAPI.Mappers;
using NoteFlowAPI.Models;
using NoteFlowAPI.Services;

namespace NoteFlowAPI.Controllers;

/// <summary>
/// Controller that handles conspect operations for the NoteFlow API.
/// </summary>
[Route("api/conspect")]
[ApiController]
public class ConspectController : ControllerBase
{
    private readonly IConspectService _conspectService;
    private readonly ILogger<ConspectController> _logger;

    /// <summary>
    /// Initializes a new instance of the ConspectController.
    /// </summary>
    /// <param name="conspectService">The service that handles conspect operations.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public ConspectController(IConspectService conspectService, ILogger<ConspectController> logger)
    {
        _conspectService = conspectService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the ID of the currently authenticated user.
    /// </summary>
    /// <returns>The user ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is missing or invalid.</exception>
    private string GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID is missing or invalid.");
        return userId;
    }

    /// <summary>
    /// Gets all conspects accessible to the current user.
    /// </summary>
    /// <param name="queryParams">Query parameters for filtering and sorting.</param>
    /// <returns>A list of accessible conspects.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ConspectQueryParams queryParams)
    {
        try
        {
            string? userId = User.Identity?.IsAuthenticated == true ? GetUserId() : null;

            var conspects = await _conspectService.GetAccessibleConspectsAsync(queryParams, userId);

            return Ok(conspects.Select(c => c.ToConspectDto()).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conspects");
            return StatusCode(
                500,
                new { message = "Error retrieving conspects", error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Gets a conspect by its ID.
    /// </summary>
    /// <param name="id">The ID of the conspect to retrieve.</param>
    /// <returns>The conspect if found and accessible.</returns>
    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        try
        {
            var conspect = await _conspectService.GetByIdAsync(id);
            if (conspect == null)
                return NotFound($"Conspect with ID '{id}' not found");

            string? userId = User.Identity?.IsAuthenticated == true ? GetUserId() : null;

            if (!_conspectService.CanUserAccessConspect(conspect, userId))
            {
                return Forbid();
            }

            return Ok(conspect.ToConspectDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conspect with ID: {Id}", id);
            return StatusCode(
                500,
                new { message = $"Error retrieving conspect with ID '{id}'", error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Creates a new conspect.
    /// </summary>
    /// <param name="conspectDTO">The conspect data to create.</param>
    /// <returns>The created conspect.</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostAsync([FromBody] CreateConspectDTO conspectDTO)
    {
        try
        {
            var conspect = conspectDTO.ToConspect();
            conspect.UserId = GetUserId();
            await _conspectService.CreateAsync(conspect);
            return CreatedAtAction(
                nameof(GetById),
                new { id = conspect.Id },
                conspect.ToConspectDto()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conspect");
            return StatusCode(500, new { message = "Error creating conspect", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing conspect.
    /// </summary>
    /// <param name="id">The ID of the conspect to update.</param>
    /// <param name="updateDTO">The updated conspect data.</param>
    /// <returns>The updated conspect.</returns>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(
        [FromRoute] string id,
        [FromBody] UpdateConspectDTO updateDTO
    )
    {
        try
        {
            var conspect = await _conspectService.GetByIdAsync(id);
            var userId = GetUserId();

            if (conspect == null)
                return NotFound($"Conspect with ID '{id}' not found");

            if (conspect.UserId != userId)
                return Forbid();

            conspect.UpdateFromDto(updateDTO);
            await _conspectService.UpdateAsync(id, conspect);
            return Ok(conspect.ToConspectDto());
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conspect with ID: {Id}", id);
            return StatusCode(500, new { message = "Error updating conspect", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates the access settings for a conspect.
    /// </summary>
    /// <param name="id">The ID of the conspect to update.</param>
    /// <param name="accessSettings">The new access settings.</param>
    /// <returns>The updated conspect.</returns>
    [HttpPut("{id}/access")]
    [Authorize]
    public async Task<IActionResult> UpdateAccess(
        [FromRoute] string id,
        [FromBody] AccessSettingsDto accessSettings
    )
    {
        try
        {
            var conspect = await _conspectService.GetByIdAsync(id);
            var userId = GetUserId();

            if (conspect == null)
                return NotFound($"Conspect with ID '{id}' not found");

            if (conspect.UserId != userId)
                return Forbid();

            conspect.IsPrivate = accessSettings.IsPrivate;

            if (accessSettings.AllowedUserIds != null && accessSettings.AllowedUserIds.Any())
            {
                conspect.AllowedUserIds = accessSettings
                    .AllowedUserIds.Where(id => !string.IsNullOrWhiteSpace(id))
                    .ToList();
            }
            else
            {
                conspect.AllowedUserIds = new List<string>();
            }

            await _conspectService.UpdateAsync(id, conspect);
            return Ok(conspect.ToConspectDto());
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating access settings for conspect with ID: {Id}", id);
            return StatusCode(
                500,
                new { message = "Error updating access settings", error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Deletes a conspect.
    /// </summary>
    /// <param name="id">The ID of the conspect to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        try
        {
            var conspect = await _conspectService.GetByIdAsync(id);

            if (conspect == null)
                return NotFound($"Conspect with ID '{id}' not found");

            if (conspect.UserId != GetUserId())
                return Forbid();

            await _conspectService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conspect with ID: {Id}", id);
            return StatusCode(500, new { message = "Error deleting conspect", error = ex.Message });
        }
    }
}
