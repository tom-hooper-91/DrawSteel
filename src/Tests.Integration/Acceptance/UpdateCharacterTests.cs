using System.Text.Json;
using API;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Integration.Acceptance;

[TestFixture]
public class UpdateCharacterTests
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

    [TestCase("Frodo", "Frodo Baggins")]
    [TestCase("Sam", "Samwise Gamgee")]
    public async Task Update_character_name_successfully(string originalName, string newName)
    {
        var createCommand = new CreateCharacterCommand(originalName);
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        var updateCommand = new UpdateCharacterCommand(characterId!, newName);
        var updateResponse = await _api.Update(characterId!.ToString(), updateCommand) as OkObjectResult;
        var updatedCharacter = JsonSerializer.Deserialize<Character>(updateResponse!.Value!.ToString()!);

        Assert.Multiple(() =>
        {
            Assert.That(updateResponse, Is.TypeOf<OkObjectResult>());
            Assert.That(updatedCharacter, Is.Not.Null);
            Assert.That(updatedCharacter!.Name, Is.EqualTo(newName));
            Assert.That(updatedCharacter.Id, Is.EqualTo(characterId));
        });
    }

    [Test]
    public async Task Return_404_when_character_not_found()
    {
        var unknownId = new CharacterId(Guid.NewGuid());
        var updateCommand = new UpdateCharacterCommand(unknownId, "New Name");

        var response = await _api.Update(unknownId.ToString(), updateCommand);

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Persist_update_across_get_requests()
    {
        var createCommand = new CreateCharacterCommand("Gandalf");
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        var updateCommand = new UpdateCharacterCommand(characterId!, "Gandalf the White");
        await _api.Update(characterId!.ToString(), updateCommand);

        var getResponse = await _api.Get(characterId.ToString()) as OkObjectResult;
        var retrievedCharacter = JsonSerializer.Deserialize<Character>(getResponse!.Value!.ToString()!);

        Assert.Multiple(() =>
        {
            Assert.That(retrievedCharacter, Is.Not.Null);
            Assert.That(retrievedCharacter!.Name, Is.EqualTo("Gandalf the White"));
            Assert.That(retrievedCharacter.Id, Is.EqualTo(characterId));
        });
    }

    [Test]
    public async Task Return_404_when_updating_deleted_character()
    {
        var createCommand = new CreateCharacterCommand("Theoden");
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        await _api.Delete(characterId!.ToString());

        var updateCommand = new UpdateCharacterCommand(characterId, "Theoden King");
        var updateResponse = await _api.Update(characterId.ToString(), updateCommand);

        Assert.That(updateResponse, Is.TypeOf<NotFoundResult>());
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("   ")]
    public async Task Return_400_when_empty_name_provided(string emptyName)
    {
        var createCommand = new CreateCharacterCommand("Eowyn");
        var createResponse = await _api.Create(createCommand) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(createResponse!.Value!.ToString()!);

        var updateCommand = new UpdateCharacterCommand(characterId!, emptyName);
        var updateResponse = await _api.Update(characterId!.ToString(), updateCommand);

        Assert.That(updateResponse, Is.TypeOf<BadRequestObjectResult>());
    }

    [TestCase("not-a-guid")]
    [TestCase("12345")]
    [TestCase("invalid")]
    public async Task Return_400_when_invalid_guid_format(string invalidGuid)
    {
        var updateCommand = new UpdateCharacterCommand(new CharacterId(Guid.NewGuid()), "Test Name");
        var updateResponse = await _api.Update(invalidGuid, updateCommand);

        Assert.That(updateResponse, Is.TypeOf<BadRequestObjectResult>());
    }
}
