using NoteFlowAPI.DTO.Conspect;
using NoteFlowAPI.Models;
using NoteFlowAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace NoteFlowAPI.Controlers;

[Microsoft.AspNetCore.Components.Route("NoteFlow/api/conspect")]
[ApiController]

public class ConspectControler : ControllerBase
{
   private readonly ConspectServices _conspectService;
   
   public ConspectControler(ConspectServices conspectService) => 
      _conspectService = conspectService;

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
   public async Task<IActionResult> GetAll() =>
      await ExecuteAsync(_conspectService.GetAsync, "Error retrieving conspect");
   
   
   [HttpGet("id/{id}")]
   public async Task<IActionResult> GetById([FromRoute] string id) =>
      await ExecuteAsync(
         () => _conspectService.GetByIdAsync(id),
         $"Conspect with ID '{id}' not found");
   
   
   [HttpPost]
   public async Task<IActionResult> PostAsync([FromBody] CreateConspectDTO conspectDTO)
   {
      try
      {
         var conspect = new Conspect()
         {
            Title = conspectDTO.Title,
            Type = conspectDTO.Type,
            Description = conspectDTO.Description,
            Text = conspectDTO.Text,
         };
         
         await _conspectService.CreateAsync(conspect);
         return CreatedAtAction(nameof(GetById), new { id = conspect.Id }, conspect);
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { message = "Error creating conspect", error = ex.Message });
      }
   }
   
   [HttpPut("{id}")]
   public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateConspectDTO updateDTO)
   {
      try
      {
         var conspect = await _conspectService.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Cannot find conspect with ID '{id}'");

         conspect.Title = updateDTO.Title;
         conspect.Type = updateDTO.Type;
         conspect.Description = updateDTO.Description;
         conspect.Text = updateDTO.Text;

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

   [HttpDelete("{id}")]
   public async Task<IActionResult> Delete([FromRoute] string id)
   {
      try
      {
         var conspect = await _conspectService.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Cannot find conspect with ID '{id}'");

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