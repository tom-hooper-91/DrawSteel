using API;
using Application;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Tests.Unit.API;

[TestFixture]
public class CreateCharacterShould
{
    [TestCase("Test")]
    [TestCase("TestTwo")]
    public void Call_CreateCharacter_action(string characterName)
    {
        var createCharacterAction = A.Fake<ICreateCharacter>();
        var createCharacterApi = new Characters(createCharacterAction);
        var command = new CreateCharacterCommand(characterName);
        var fakeRequest = A.Fake<HttpRequest>();
        fakeRequest.Body = new StringContent(JsonSerializer.Serialize(new { Name = characterName })).ReadAsStream();
        
        createCharacterApi.Create(fakeRequest);

        A.CallTo(() => createCharacterAction.Execute(command)).MustHaveHappened();
    }
}