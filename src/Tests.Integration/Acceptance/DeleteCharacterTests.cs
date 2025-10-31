using System.Text.Json;
using API;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Integration.Acceptance;

[TestFixture]
public class DeleteCharacterTests
{
    private MongoDbCharacterRepository _repository;
    private Characters _api;

    [SetUp]
    public void Setup()
    {
        _repository = new MongoDbCharacterRepository(Fixture.Client);
        var service = new CharacterService(_repository);
        var create = new CreateCharacter(service);
        var get = new GetCharacter(service);
        var update = new UpdateCharacter(service);
        var delete = new DeleteCharacter(service);
        _api = new Characters(create, get, update, delete);
    }

    // T039: Delete character successfully
    [Test]
    public async Task Delete_character_successfully()
    {
        // Create a character first
        var createCommand = new CreateCharacterCommand("Boromir");
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        // Delete the character
        var deleteResponse = await _api.Delete(characterId!.ToString()) as OkObjectResult;

        Assert.That(deleteResponse, Is.Not.Null);
    }

    // T040: Character not retrievable after delete
    [Test]
    public async Task Character_not_retrievable_after_delete()
    {
        // Create a character
        var createCommand = new CreateCharacterCommand("Saruman");
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        // Delete the character
        await _api.Delete(characterId!.ToString());

        // Try to retrieve the character
        var getResponse = await _api.Get(characterId.ToString());

        Assert.That(getResponse, Is.TypeOf<NotFoundResult>());
    }

    // T041: Delete returns success when already deleted
    [Test]
    public async Task Delete_returns_success_when_already_deleted()
    {
        // Create a character
        var createCommand = new CreateCharacterCommand("Gollum");
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        // Delete the character first time
        var firstDeleteResponse = await _api.Delete(characterId!.ToString()) as OkObjectResult;

        // Delete the character second time (idempotent)
        var secondDeleteResponse = await _api.Delete(characterId.ToString()) as OkObjectResult;

        Assert.Multiple(() =>
        {
            Assert.That(firstDeleteResponse, Is.Not.Null);
            Assert.That(secondDeleteResponse, Is.Not.Null);
        });
    }
}
