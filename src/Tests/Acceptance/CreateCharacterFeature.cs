using Application;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CreateCharacter = API.CreateCharacter;

namespace Tests.Acceptance;

[TestFixture]
public class CreateCharacterFeature
{
    [Test]
    public void Return_a_success()
    {
        var request = A.Fake<HttpRequest>();
        var createCharacterAction = new Application.CreateCharacter();
        var createCharacter = new CreateCharacter(createCharacterAction);
        
        Assert.That(createCharacter.Run(request), Is.TypeOf<OkObjectResult>());
    }
}