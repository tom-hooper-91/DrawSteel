using API;
using API.Contracts;
using API.Requests;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Integration.Acceptance;

[TestFixture]
public class UpdateCharacterTests
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

    [TestCase("Frodo", "Frodo Baggins")]
    [TestCase("Sam", "Samwise Gamgee")]
    public async Task Update_character_name_successfully(string originalName, string newName)
    {
        var createRequest = new CharacterRequest { Name = originalName };
        var createResponse = await _api.Create(createRequest) as OkObjectResult;
        var createdCharacter = createResponse?.Value as CharacterResponse;
        Assert.That(createdCharacter, Is.Not.Null);
        var characterId = new CharacterId(Guid.Parse(createdCharacter!.Id));

        var updateRequest = new CharacterRequest { Id = characterId.Value.ToString(), Name = newName };
        var updateResponse = await _api.Update(characterId.Value, updateRequest) as OkObjectResult;
        var updatedCharacter = updateResponse?.Value as CharacterResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updateResponse, Is.TypeOf<OkObjectResult>());
            Assert.That(updatedCharacter, Is.Not.Null);
            Assert.That(updatedCharacter!.Name, Is.EqualTo(newName));
            Assert.That(Guid.Parse(updatedCharacter.Id), Is.EqualTo(characterId.Value));
        });
    }

    [Test]
    public async Task Return_404_when_character_not_found()
    {
        var unknownId = new CharacterId(Guid.NewGuid());
        var request = new CharacterRequest { Id = unknownId.Value.ToString(), Name = "New Name" };

        var response = await _api.Update(unknownId.Value, request);

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Persist_update_across_get_requests()
    {
        var createRequest = new CharacterRequest { Name = "Gandalf" };
        var createResponse = await _api.Create(createRequest) as OkObjectResult;
        var createdCharacter = createResponse?.Value as CharacterResponse;
        Assert.That(createdCharacter, Is.Not.Null);
        var characterId = new CharacterId(Guid.Parse(createdCharacter!.Id));

        var updateRequest = new CharacterRequest { Id = characterId.Value.ToString(), Name = "Gandalf the White" };
        await _api.Update(characterId.Value, updateRequest);

        var getResponse = await _api.Get(characterId.Value) as OkObjectResult;
        var retrievedCharacter = getResponse?.Value as CharacterResponse;

        Assert.Multiple(() =>
        {
            Assert.That(retrievedCharacter, Is.Not.Null);
            Assert.That(retrievedCharacter!.Name, Is.EqualTo("Gandalf the White"));
            Assert.That(Guid.Parse(retrievedCharacter.Id), Is.EqualTo(characterId.Value));
        });
    }

    [Test]
    public async Task Return_404_when_updating_deleted_character()
    {
        var createRequest = new CharacterRequest { Name = "Theoden" };
        var createResponse = await _api.Create(createRequest) as OkObjectResult;
        var createdCharacter = createResponse?.Value as CharacterResponse;
        Assert.That(createdCharacter, Is.Not.Null);
        var characterId = new CharacterId(Guid.Parse(createdCharacter!.Id));

        await _api.Delete(characterId.Value);

        var updateRequest = new CharacterRequest { Id = characterId.Value.ToString(), Name = "Theoden King" };
        var updateResponse = await _api.Update(characterId.Value, updateRequest);

        Assert.That(updateResponse, Is.TypeOf<NotFoundResult>());
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("   ")]
    public async Task Return_400_when_empty_name_provided(string emptyName)
    {
        var createRequest = new CharacterRequest { Name = "Eowyn" };
        var createResponse = await _api.Create(createRequest) as OkObjectResult;
        var createdCharacter = createResponse?.Value as CharacterResponse;
        Assert.That(createdCharacter, Is.Not.Null);
        var characterId = new CharacterId(Guid.Parse(createdCharacter!.Id));

        var updateRequest = new CharacterRequest { Id = characterId.Value.ToString(), Name = emptyName };
        var updateResponse = await _api.Update(characterId.Value, updateRequest);

        Assert.That(updateResponse, Is.TypeOf<BadRequestObjectResult>());
    }

    // Route binding enforces GUID parsing before controller logic is invoked, so invalid
    // GUID scenarios are covered by API-level tests instead of controller invocations.
}
