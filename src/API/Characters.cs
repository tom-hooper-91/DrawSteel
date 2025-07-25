using System.Text.Json;
using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API;

[ApiController]
[Route("api/[controller]")]
public class Characters(ICreateCharacter createCharacter, IGetCharacter getCharacter) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCharacterCommand command)
    {
        var characterId = await createCharacter.Execute(command);
        
        return new OkObjectResult(JsonSerializer.Serialize(characterId));
    }

    [HttpGet("{characterId}")]
    public async Task<IActionResult> Get([FromRoute] string characterId) 
    {
        var id = new CharacterId(new Guid(characterId));
        var character = await getCharacter.Execute(id);
            
        return new OkObjectResult(JsonSerializer.Serialize(character));
    }
}