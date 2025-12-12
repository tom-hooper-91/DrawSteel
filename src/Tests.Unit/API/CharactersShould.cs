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
    private IUpdateCharacter _updateCharacter;
    private IDeleteCharacter _deleteCharacter;

    [SetUp]
    public void Setup()
    {
        _createCharacter = A.Fake<ICreateCharacter>();
        _getCharacter = A.Fake<IGetCharacter>();
        _updateCharacter = A.Fake<IUpdateCharacter>();
        _deleteCharacter = A.Fake<IDeleteCharacter>();
        _api = new Characters(_createCharacter, _getCharacter, _updateCharacter, _deleteCharacter);
    }

    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Return_serialised_characterId_on_post(string name)
    {
        var newCharacter = new CreateCharacterRequest(name, CharacterClass.Warrior);
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
        var badCharacter = new CreateCharacterRequest("Something broken", CharacterClass.Warrior);
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

    [TestCase("Frodo", "Frodo Baggins")]
    [TestCase("Sam", "Samwise Gamgee")]
    public async Task Return_200_with_updated_character_on_successful_update(string oldName, string newName)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var command = new UpdateCharacterCommand(characterId, newName);
        var updatedCharacter = new Character(characterId, newName);
        A.CallTo(() => _updateCharacter.Execute(A<UpdateCharacterCommand>._)).Returns(updatedCharacter);

        var response = await _api.Update(characterId.ToString(), command) as OkObjectResult;
        var returnedCharacter = JsonSerializer.Deserialize<Character>(response!.Value!.ToString()!);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(returnedCharacter, Is.EqualTo(updatedCharacter));
        });
    }

    [Test]
    public async Task Return_404_when_updating_nonexistent_character()
    {
        var unknownId = new CharacterId(Guid.NewGuid());
        var command = new UpdateCharacterCommand(unknownId, "New Name");
        A.CallTo(() => _updateCharacter.Execute(A<UpdateCharacterCommand>._)).Returns(Task.FromResult<Character?>(null));

        var result = await _api.Update(unknownId.ToString(), command);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [TestCase("not-a-guid")]
    [TestCase("12345")]
    [TestCase("")]
    public async Task Return_400_when_updating_with_invalid_guid(string invalidGuid)
    {
        var command = new UpdateCharacterCommand(new CharacterId(Guid.NewGuid()), "New Name");

        var result = await _api.Update(invalidGuid, command) as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("   ")]
    public async Task Return_400_when_updating_with_empty_name(string emptyName)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var command = new UpdateCharacterCommand(characterId, emptyName);
        A.CallTo(() => _updateCharacter.Execute(A<UpdateCharacterCommand>._))
            .Throws(new ArgumentException("Character name cannot be empty"));

        var result = await _api.Update(characterId.ToString(), command) as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Return_200_on_successful_delete()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _deleteCharacter.Execute(characterId)).Returns(true);

        var result = await _api.Delete(characterId.ToString()) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Return_200_on_idempotent_delete()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _deleteCharacter.Execute(characterId)).Returns(false);

        var result = await _api.Delete(characterId.ToString()) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
    }

    [TestCase("not-a-guid")]
    [TestCase("12345")]
    [TestCase("")]
    public async Task Return_400_when_deleting_with_invalid_guid(string invalidGuid)
    {
        var result = await _api.Delete(invalidGuid) as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
    }
}