using API;
using Application;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Acceptance;

[TestFixture]
public class CreateCharacterFeature
{
    [Test]
    public void Return_a_success()
    {
        var request = A.Fake<HttpRequest>();
        var createCharacterAction = new Application.CreateCharacter();
        var createCharacter = new Characters(createCharacterAction);
        
        Assert.That(createCharacter.Create(request), Is.TypeOf<OkObjectResult>());
    }
}