using System.Text;
using API;
using Application;
using Domain;
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
        var createCharacter = new Characters(createCharacterAction);
        
        Assert.That(createCharacter.Create(httpContext.Request), Is.TypeOf<OkObjectResult>());
    }
}