using API;
using API.Contracts;
using API.Requests;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Integration.Acceptance;

[TestFixture]
public class CharacterFeatures
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
    public async Task Create_a_character_in_the_database_and_respond_with_the_characters_id()
    {
        var frodo = new CharacterRequest { Name = "Frodo" };

        var response = await _api.Create(frodo) as OkObjectResult;
        var payload = response?.Value as CharacterResponse;
        Assert.That(payload, Is.Not.Null, "Create endpoint must return a CharacterResponse payload.");

        var savedCharacter = await _repository.Get(new CharacterId(Guid.Parse(payload!.Id)));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            Assert.That(savedCharacter.Id.Value.ToString(), Is.EqualTo(payload.Id));
            Assert.That(savedCharacter.Name, Is.EqualTo(frodo.Name));
        });
    }

    [Test]
    public async Task Respond_with_an_existing_character_from_the_database()
    {
        var existingCharacterId = new CharacterId(Guid.NewGuid());
        var existingCharacter = new Character(existingCharacterId, "Sam");
        await _repository.Add(existingCharacter);

        var response = await _api.Get(existingCharacterId.Value) as OkObjectResult;
        var returnedCharacter = response?.Value as CharacterResponse;

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            Assert.That(returnedCharacter, Is.Not.Null);
            Assert.That(Guid.Parse(returnedCharacter!.Id), Is.EqualTo(existingCharacterId.Value));
            Assert.That(returnedCharacter.Name, Is.EqualTo(existingCharacter.Name));
        });
    }
}