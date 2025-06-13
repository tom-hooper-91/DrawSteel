using System.Text.Json;
using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API;

[ApiController]
[Route("api/[controller]")]
public class Characters(ICreateCharacter createCharacter) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCharacterCommand command)
    {
        var characterId = await createCharacter.Execute(command);
        
        return new OkObjectResult(JsonSerializer.Serialize(characterId));
    }
}