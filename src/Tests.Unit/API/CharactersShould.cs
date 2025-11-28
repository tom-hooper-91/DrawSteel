using System.Text.Json;
using API;
using API.Requests;
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
    private IListCharacters _listCharacters;

    [SetUp]
    public void Setup()
    {
        _createCharacter = A.Fake<ICreateCharacter>();
        _getCharacter = A.Fake<IGetCharacter>();
        _updateCharacter = A.Fake<IUpdateCharacter>();
        _deleteCharacter = A.Fake<IDeleteCharacter>();
        _listCharacters = A.Fake<IListCharacters>();
        _api = new Characters(_createCharacter, _getCharacter, _updateCharacter, _deleteCharacter, _listCharacters);
    }

    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Return_flat_characterId_on_post(string name)
    {
        var expectedCharacterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _createCharacter.Execute(A<CreateCharacterInput>.That.Matches(input => input.Name == name)))
            .Returns(expectedCharacterId);

        var request = new CharacterRequest { Name = name };

        var response = await _api.Create(request) as OkObjectResult;
        var payload = response.ExtractPayload();

        Assert.Multiple(() =>
        {
            Assert.That(payload.TryGetProperty("id", out var idProperty), Is.True, "Response must include flattened 'id'.");
            Assert.That(Guid.Parse(idProperty.GetString() ?? string.Empty), Is.EqualTo(expectedCharacterId.Value));
        });

        A.CallTo(() => _createCharacter.Execute(A<CreateCharacterInput>.That.Matches(input => input.Name == name)))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task Return_bad_request_when_CreateCharacter_throws()
    {
        var badCharacter = new CharacterRequest { Name = "Something broken" };
        A.CallTo(() => _createCharacter.Execute(A<CreateCharacterInput>._)).Throws(new Exception("This went wrong"));

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
        var expectedDto = new CharacterDto(existingCharacterId.Value.ToString(), name);
        A.CallTo(() => _getCharacter.Execute(existingCharacterId)).Returns(Task.FromResult<CharacterDto?>(expectedDto));

        var response = await _api.Get(existingCharacterId.Value) as OkObjectResult;
        var payload = response.ExtractPayload();

        Assert.Multiple(() =>
        {
            Assert.That(payload.TryGetProperty("id", out var idProperty), Is.True);
            Assert.That(Guid.Parse(idProperty.GetString() ?? string.Empty), Is.EqualTo(existingCharacterId.Value));
            Assert.That(payload.GetProperty("name").GetString(), Is.EqualTo(name));
        });
    }

    [Test]
    public async Task Return_not_found_result_when_Character_does_not_exist()
    {
        var unknownId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _getCharacter.Execute(unknownId))!.Returns(Task.FromResult<CharacterDto?>(null));

        var result = await _api.Get(unknownId.Value);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [TestCase("Frodo", "Frodo Baggins")]
    [TestCase("Sam", "Samwise Gamgee")]
    public async Task Return_200_with_updated_character_on_successful_update(string oldName, string newName)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var updatedCharacter = new CharacterDto(characterId.Value.ToString(), newName);
        A.CallTo(() => _updateCharacter.Execute(A<UpdateCharacterInput>.That.Matches(input =>
                input.RouteId == characterId.Value &&
                input.PayloadId == characterId.Value.ToString() &&
                input.Name == newName)))
            .Returns(Task.FromResult<CharacterDto?>(updatedCharacter));

        var request = new CharacterRequest { Id = characterId.Value.ToString(), Name = newName };

        var response = await _api.Update(characterId.Value, request) as OkObjectResult;
        var payload = response.ExtractPayload();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(payload.TryGetProperty("id", out var idProperty), Is.True);
            Assert.That(Guid.Parse(idProperty.GetString() ?? string.Empty), Is.EqualTo(characterId.Value));
            Assert.That(payload.GetProperty("name").GetString(), Is.EqualTo(updatedCharacter.Name));
        });
    }

    [Test]
    public async Task Return_404_when_updating_nonexistent_character()
    {
        var unknownId = new CharacterId(Guid.NewGuid());
        var request = new CharacterRequest { Id = unknownId.Value.ToString(), Name = "New Name" };
        A.CallTo(() => _updateCharacter.Execute(A<UpdateCharacterInput>._)).Returns(Task.FromResult<CharacterDto?>(null));

        var result = await _api.Update(unknownId.Value, request);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("   ")]
    public async Task Return_400_when_updating_with_empty_name(string emptyName)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var request = new CharacterRequest { Id = characterId.Value.ToString(), Name = emptyName };
        A.CallTo(() => _updateCharacter.Execute(A<UpdateCharacterInput>._))
            .Throws(new ArgumentException("Character name cannot be empty"));

        var result = await _api.Update(characterId.Value, request) as ObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    public async Task Return_200_on_successful_delete()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _deleteCharacter.Execute(characterId)).Returns(true);

        var result = await _api.Delete(characterId.Value) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Return_200_on_idempotent_delete()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _deleteCharacter.Execute(characterId)).Returns(false);

        var result = await _api.Delete(characterId.Value) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
    }

}

internal static class JsonPayloadExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };

    public static JsonElement ExtractPayload(this ObjectResult? result)
    {
        Assert.That(result, Is.Not.Null);
        var raw = result!.Value switch
        {
            string json => json,
            _ => JsonSerializer.Serialize(result.Value, SerializerOptions)
        };

        using var document = JsonDocument.Parse(raw);
        return document.RootElement.Clone();
    }
}