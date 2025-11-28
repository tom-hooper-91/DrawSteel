using API;
using API.Contracts;
using API.Requests;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Integration.Acceptance;

[TestFixture]
public class DeleteCharacterTests
{
    private MongoDbCharacterRepository _repository = null!;
    private Characters _api = null!;

    [SetUp]
    public async Task Setup()
    {
        _repository = new MongoDbCharacterRepository(CharacterApiFixture.MongoClient);
        var service = new CharacterService(_repository);
        var create = new CreateCharacter(service);
        var get = new GetCharacter(service);
        var update = new UpdateCharacter(service);
        var delete = new DeleteCharacter(service);
        var list = new ListCharacters(service);
        _api = new Characters(create, get, update, delete, list);

        await CharacterApiFixture.ClearCharactersAsync();
    }

    [Test]
    public async Task Delete_character_successfully()
    {
        var createRequest = new CharacterRequest { Name = "Boromir" };
        var createResponse = await _api.Create(createRequest) as OkObjectResult;
        var createdCharacter = createResponse?.Value as CharacterResponse;
        Assert.That(createdCharacter, Is.Not.Null);
        var characterId = new CharacterId(Guid.Parse(createdCharacter!.Id));

        var deleteResponse = await _api.Delete(characterId.Value) as OkObjectResult;

        Assert.That(deleteResponse, Is.Not.Null);
    }

    [Test]
    public async Task Character_not_retrievable_after_delete()
    {
        var createRequest = new CharacterRequest { Name = "Saruman" };
        var createResponse = await _api.Create(createRequest) as OkObjectResult;
        var createdCharacter = createResponse?.Value as CharacterResponse;
        Assert.That(createdCharacter, Is.Not.Null);
        var characterId = new CharacterId(Guid.Parse(createdCharacter!.Id));

        await _api.Delete(characterId.Value);

        var getResponse = await _api.Get(characterId.Value);

        Assert.That(getResponse, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_returns_success_when_already_deleted()
    {
        var createRequest = new CharacterRequest { Name = "Gollum" };
        var createResponse = await _api.Create(createRequest) as OkObjectResult;
        var createdCharacter = createResponse?.Value as CharacterResponse;
        Assert.That(createdCharacter, Is.Not.Null);
        var characterId = new CharacterId(Guid.Parse(createdCharacter!.Id));

        var firstDeleteResponse = await _api.Delete(characterId.Value) as OkObjectResult;

        var secondDeleteResponse = await _api.Delete(characterId.Value) as OkObjectResult;

        Assert.Multiple(() =>
        {
            Assert.That(firstDeleteResponse, Is.Not.Null);
            Assert.That(secondDeleteResponse, Is.Not.Null);
        });
    }
}
