using System.Text.Json;
using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API;

[ApiController]
[Route("api/[controller]")]
public class Characters(
    ICreateCharacter createCharacter, 
    IGetCharacter getCharacter,
    IUpdateCharacter updateCharacter,
    IDeleteCharacter deleteCharacter) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCharacterCommand command)
    {
        try
        {
            var characterId = await createCharacter.Execute(command);
            return new OkObjectResult(JsonSerializer.Serialize(characterId));
        }
        catch
        {
            return Problem();
        }
    }

    [HttpGet("{characterId}")]
    public async Task<IActionResult> Get([FromRoute] string characterId)
    {
        var id = new CharacterId(new Guid(characterId));
        var character = await getCharacter.Execute(id);

        if (character is null)
            return new NotFoundResult();

        return new OkObjectResult(JsonSerializer.Serialize(character));
    }

    // T025: Add PUT endpoint
    [HttpPut("{characterId}")]
    public async Task<IActionResult> Update([FromRoute] string characterId, [FromBody] UpdateCharacterCommand command)
    {
        try
        {
            // Validate GUID format
            if (!Guid.TryParse(characterId, out var guid))
                return BadRequest(new { error = "Invalid character ID format", statusCode = 400 });
            
            var id = new CharacterId(guid);
            var updatedCommand = new UpdateCharacterCommand(id, command.Name);
            
            var character = await updateCharacter.Execute(updatedCommand);
            
            // Null means not found
            if (character is null)
                return NotFound();
            
            return Ok(JsonSerializer.Serialize(character));
        }
        catch (ArgumentException ex)
        {
            // Business rule violations (e.g., empty name)
            return BadRequest(new { error = ex.Message, statusCode = 400 });
        }
        catch
        {
            return Problem();
        }
    }

    [HttpDelete("{characterId}")]
    public async Task<IActionResult> Delete([FromRoute] string characterId)
    {
        try
        {
            // Validate GUID format
            if (!Guid.TryParse(characterId, out var guid))
                return BadRequest(new { error = "Invalid character ID format", statusCode = 400 });
            
            var id = new CharacterId(guid);
            await deleteCharacter.Execute(id);
            
            // Always return success (idempotent)
            return Ok(new { message = "Character deleted successfully", characterId = id.Value });
        }
        catch
        {
            return Problem();
        }
    }
}