using System.Text;
using System.Text.Json;
using API;
using Application;
using Domain;
using Domain.Repositories;
using FakeItEasy;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Acceptance;

[TestFixture]
public class CreateCharacterFeature
{
    [Test]
    public void Return_a_success()
    {
        var httpContext = new DefaultHttpContext();
        var stringData = Encoding.Default.GetBytes("{\n\"name\":\"Frodo\"\n}");
        httpContext.Request.ContentLength = stringData.Length;
        httpContext.Request.ContentType = "application/json";
        httpContext.Request.Body = new MemoryStream(stringData);
        
        var characterFactory = new CharacterFactory();
        var characterRepository = new InMemoryCharacterRepository();
        var createCharacterAction = new CreateCharacter(characterFactory, new SaveCharacter(characterRepository));
        var characters = new Characters(createCharacterAction);

        var actionResult = characters.Create(httpContext.Request) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(actionResult!.Value!.ToString()!);
        var frodo = characterRepository.Get(characterId!);
        
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());
            Assert.That(frodo.Name, Is.EqualTo("Frodo"));
        });
    }
}