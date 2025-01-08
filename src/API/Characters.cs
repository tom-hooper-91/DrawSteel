using System.Text.Json;
using Application;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace API;

public class Characters(ICreateCharacter CreateCharacter)
{
    
    [Function("characters")]
    public IActionResult Create([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        var jsonString = new StreamReader(req.Body).ReadToEnd();
        var command = JsonSerializer.Deserialize<CreateCharacterCommand>(jsonString);
        
        if (command is null) 
            return new BadRequestObjectResult("Please pass a request body");
        
        CreateCharacter.Execute(command);
        
        return new OkObjectResult("OK!");
    }
}