using System.Text.Json;
using API;
using Application;
using Domain;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

[TestFixture]
public class CharactersShould
{
    private Characters _api;
    private ICreateCharacter _createCharacter;

    [SetUp]
    public void Setup()
    {
        _createCharacter = A.Fake<ICreateCharacter>();
        _api = new Characters(_createCharacter);
    }

    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Call_CreateCharacter_action_on_post(string name)
    {
        var newCharacter = new CreateCharacterCommand(name);
        var expectedCharacterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _createCharacter.Execute(newCharacter)).Returns(expectedCharacterId);

        var response = await _api.Create(newCharacter) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(response!.Value!.ToString()!);

        A.CallTo(() => _createCharacter.Execute(newCharacter)).MustHaveHappened();
        Assert.That(characterId, Is.EqualTo(expectedCharacterId));
    }
}