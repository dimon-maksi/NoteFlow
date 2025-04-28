using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteFlowAPI.DTO.Conspect;
using NoteFlowAPI.Helpers.QueryParam;
using NoteFlowAPI.Mappers;
using NoteFlowAPI.Models;
using NoteFlowAPI.Services;

namespace NoteFlowAPI.Controlers;

[Route("api/conspect")]
[ApiController]
public class ConspectControler : ControllerBase
{
    private readonly ConspectServices _conspectService;

    public ConspectControler(ConspectServices conspectService) =>
        _conspectService = conspectService;

    public string GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID is missing or invalid.");
        return userId;
    }

    private async Task<IActionResult> ExecuteAsync<T>(Func<Task<T>> action, string errorMessage)
    {
        try
        {
            var result = await action();
            if (result == null || (result is IEnumerable<Conspect> conspect && !conspect.Any()))
                return NotFound(errorMessage);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = errorMessage, error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ConspectQueryParams queryParams) =>
        await ExecuteAsync(
            async () =>
                (await _conspectService.GetAsync(queryParams))
                    .Select(c => c.ToConspectDto())
                    .ToList(),
            "Error retrieving conspects"
        );

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id) =>
        await ExecuteAsync(
            async () => (await _conspectService.GetByIdAsync(id))?.ToConspectDto(),
            $"Conspect with ID '{id}' not found"
        );

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostAsync([FromBody] CreateConspectDTO conspectDTO)
    {
        try
        {
            var conspect = conspectDTO.ToConspect();
            conspect.UserId = GetUserId();
            await _conspectService.CreateAsync(conspect);
            return CreatedAtAction(nameof(GetById), new { id = conspect.Id }, conspect);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating conspect", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] string id,
        [FromBody] UpdateConspectDTO updateDTO
    )
    {
        try
        {
            var conspect = await _conspectService.GetByIdAsync(id);
            var userId = GetUserId();
            if (conspect == null || conspect.UserId != userId)
                return Forbid();

            conspect.UpdateFromDto(updateDTO);
            await _conspectService.UpdateAsync(id, conspect);
            return Ok(conspect);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating conspect", error = ex.Message });
        }
    }

    [HttpPut("{id}/access")]
    [Authorize]
    public async Task<IActionResult> UpdateAcess(string id, AccessSettingsDto acessSettings)
    {
        // TODO: finish logic for checking if user owns or has acess to docs
        //
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ConspectQueryParams queryParams)
    {
        var userId = User.Identity.IsAuthenticated ? GetUserId() : null;
        //TODO: finish this shit with repo
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        try
        {
            var conspect = await _conspectService.GetByIdAsync(id);
            if (conspect == null || conspect.UserId != GetUserId())
                return Forbid();

            await _conspectService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting conspect", error = ex.Message });
        }
    }
}

