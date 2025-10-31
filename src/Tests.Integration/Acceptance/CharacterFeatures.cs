using System.Text.Json;
using API;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Integration.Acceptance;

[TestFixture]
public class CharacterFeatures
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

    [Test]
    public async Task Create_a_character_in_the_database_and_respond_with_the_characters_id()
    {
        var frodo = new CreateCharacterCommand("Frodo");

        var response = await _api.Create(frodo) as OkObjectResult;
        var frodoId = JsonSerializer.Deserialize<CharacterId>(response!.Value!.ToString()!);
        var savedCharacter = await _repository.Get(frodoId!);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            Assert.That(frodoId, Is.Not.Null);
            Assert.That(savedCharacter.Id, Is.EqualTo(frodoId));
            Assert.That(savedCharacter.Name, Is.EqualTo(frodo.Name));
        });
    }

    [Test]
    public async Task Respond_with_an_existing_character_from_the_database()
    {
        var existingCharacterId = new CharacterId(Guid.NewGuid());
        var existingCharacter = new Character(existingCharacterId, "Sam");
        await _repository.Add(existingCharacter);

        var response = await _api.Get(existingCharacterId.ToString()) as OkObjectResult;
        var returnedCharacter = JsonSerializer.Deserialize<Character>(response!.Value!.ToString()!);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            Assert.That(returnedCharacter, Is.Not.Null);
            Assert.That(returnedCharacter!.Id, Is.EqualTo(existingCharacterId));
            Assert.That(returnedCharacter.Name, Is.EqualTo(existingCharacter.Name));
        });
    }
}