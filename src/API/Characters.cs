using Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace API;

public class Characters(ICreateCharacter CreateCharacter)
{
    
    [Function("characters")]
    public IActionResult Create([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        CreateCharacter.Execute();
        return new OkObjectResult("OK!");
    }
}