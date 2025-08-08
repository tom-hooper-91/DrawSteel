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

    [Test]
    public async Task Return_bad_request_when_CreateCharacter_throws()
    {
        var badCharacter = new CreateCharacterCommand("Something broken");
        A.CallTo(() => _createCharacter.Execute(badCharacter)).Throws(new Exception("This went wrong"));

        var response = await _api.Create(badCharacter) as ObjectResult;

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.StatusCode, Is.EqualTo(500));
        });
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

    [Test]
    public async Task Return_not_found_result_when_Character_does_not_exist()
    {
        var unknownId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _getCharacter.Execute(unknownId))!.Returns(Task.FromResult<Character>(null!));

        var result = await _api.Get(unknownId.ToString());

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}