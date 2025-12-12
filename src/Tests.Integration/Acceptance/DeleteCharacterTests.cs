using System.Text.Json;
using API;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

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
        _api = new Characters(create, get, update, delete, NullLogger<Characters>.Instance);
    }

    [Test]
    public async Task Delete_character_successfully()
    {
        var createCommand = new CreateCharacterRequest("Boromir", CharacterClass.Tactician);
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        var deleteResponse = await _api.Delete(characterId!.ToString()) as OkObjectResult;

        Assert.That(deleteResponse, Is.Not.Null);
    }

    [Test]
    public async Task Character_not_retrievable_after_delete()
    {
        var createCommand = new CreateCharacterRequest("Saruman", CharacterClass.Tactician);
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        await _api.Delete(characterId!.ToString());

        var getResponse = await _api.Get(characterId.ToString());

        Assert.That(getResponse, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_returns_success_when_already_deleted()
    {
        var createCommand = new CreateCharacterRequest("Gollum", CharacterClass.Tactician);
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        var firstDeleteResponse = await _api.Delete(characterId!.ToString()) as OkObjectResult;

        var secondDeleteResponse = await _api.Delete(characterId.ToString()) as OkObjectResult;

        Assert.Multiple(() =>
        {
            Assert.That(firstDeleteResponse, Is.Not.Null);
            Assert.That(secondDeleteResponse, Is.Not.Null);
        });
    }
}
