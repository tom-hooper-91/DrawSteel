using System.Text.Json;
using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class Characters(ICreateCharacter createCharacter)
{
    public async Task<IActionResult> Create(HttpRequest req)
    {
        var jsonString = await new StreamReader(req.Body).ReadToEndAsync();
        var command = JsonSerializer.Deserialize<CreateCharacterCommand>(jsonString, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        
        if (command is null) 
            return new BadRequestObjectResult("Please pass a request body");
        
        var characterId = await createCharacter.Execute(command);
        
        return new OkObjectResult(JsonSerializer.Serialize(characterId));
    }
}