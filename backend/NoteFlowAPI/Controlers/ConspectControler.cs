using NoteFlowAPI.DTO.Conspect;
using NoteFlowAPI.Models;
using NoteFlowAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace NoteFlowAPI.Controlers;

[Microsoft.AspNetCore.Components.Route("NoteFlow/api/conspect")]
[ApiController]

public class ConspectControler : ControllerBase
{
   private readonly ConspectsServices _conspectsService;
   
   public ConspectControler(ConspectsServices conspectsService) => 
      _conspectsService = conspectsService;

   private async Task<IActionResult> ExecuteAsync<T>(Func<Task<T>> action, string errorMessage)
   {
      try
      {
         var result = await action();
         if (result == null || (result is IEnumerable<Conspects> conspects && !conspects.Any()))
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
      await ExecuteAsync(_conspectsService.GetAsync, "Error retrieving conspects");
   
   
   [HttpGet("id/{id}")]
   public async Task<IActionResult> GetById([FromRoute] string id) =>
      await ExecuteAsync(
         () => _conspectsService.GetByIdAsync(id),
         $"Conspect with ID '{id}' not found");
   
   
   [HttpPost]
   public async Task<IActionResult> PostAsync([FromBody] CreateConspectDTO conspectDTO)
   {
      try
      {
         var conspects = new Conspects()
         {
            Title = conspectDTO.Title,
            Type = conspectDTO.Type,
            Description = conspectDTO.Description,
            Text = conspectDTO.Text,
         };
         
         await _conspectsService.CreateAsync(conspects);
         return CreatedAtAction(nameof(GetById), new { id = conspects.Id }, conspects);
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
         var conspects = await _conspectsService.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Cannot find conspect with ID '{id}'");

         conspects.Title = updateDTO.Title;
         conspects.Type = updateDTO.Type;
         conspects.Description = updateDTO.Description;
         conspects.Text = updateDTO.Text;

         await _conspectsService.UpdateAsync(id, conspects);
         return Ok(conspects);
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
         var conspects = await _conspectsService.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Cannot find conspect with ID '{id}'");

         await _conspectsService.DeleteAsync(id);
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