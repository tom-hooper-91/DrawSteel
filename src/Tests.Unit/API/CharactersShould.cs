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
    private IGetCharacter _getCharacter;

    [SetUp]
    public void Setup()
    {
        _createCharacter = A.Fake<ICreateCharacter>();
        _getCharacter = A.Fake<IGetCharacter>();
        _api = new Characters(_createCharacter, _getCharacter);
    }

    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Return_serialised_characterId_on_post(string name)
    {
        var newCharacter = new CreateCharacterCommand(name);
        var expectedCharacterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _createCharacter.Execute(newCharacter)).Returns(expectedCharacterId);

        var response = await _api.Create(newCharacter) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(response!.Value!.ToString()!);

        A.CallTo(() => _createCharacter.Execute(newCharacter)).MustHaveHappenedOnceExactly();
        Assert.That(characterId, Is.EqualTo(expectedCharacterId));
    }

    
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Return_serialised_character_on_get(string name)
    {
        var existingCharacterId = new CharacterId(Guid.NewGuid());
        var existingCharacter = new Character(existingCharacterId, name);
        A.CallTo(() => _getCharacter.Execute(existingCharacterId)).Returns(existingCharacter);
        
        var response = await _api.Get(existingCharacterId.ToString()) as OkObjectResult;
        var returnedCharacter = JsonSerializer.Deserialize<Character>(response!.Value!.ToString()!);
        
        Assert.That(returnedCharacter, Is.EqualTo(existingCharacter));
    }
}