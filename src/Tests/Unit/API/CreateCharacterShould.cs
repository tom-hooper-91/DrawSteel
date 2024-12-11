using API;
using Application;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Tests.Unit.API;

[TestFixture]
public class CreateCharacterShould
{
    private Characters _createCharacterApi;
    private ICreateCharacter _createCharacterAction;

    [SetUp]
    public void Setup()
    {
        _createCharacterAction = A.Fake<ICreateCharacter>();
        _createCharacterApi = new Characters(_createCharacterAction);
    }
    
    [TestCase("Test")]
    [TestCase("TestTwo")]
    public void Call_CreateCharacter_action(string characterName)
    {
        var command = new CreateCharacterCommand(characterName);
        var fakeRequest = GivenApiRequestWithValidCharacterName(characterName);

        _createCharacterApi.Create(fakeRequest);

        A.CallTo(() => _createCharacterAction.Execute(command)).MustHaveHappened();
    }

    private static HttpRequest GivenApiRequestWithValidCharacterName(string characterName)
    {
        var fakeRequest = A.Fake<HttpRequest>();
        fakeRequest.Body = new StringContent(JsonSerializer.Serialize(new { Name = characterName })).ReadAsStream();
        return fakeRequest;
    }
}