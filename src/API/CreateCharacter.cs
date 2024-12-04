using Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace API;

public class CreateCharacter(ICreateCharacter CreateCharacter)
{
    
    [Function("/characters")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        CreateCharacter.Execute();
        return new OkObjectResult("OK!");
    }
}