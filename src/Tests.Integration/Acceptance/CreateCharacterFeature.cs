using System.Text.Json;
using API;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Integration.Acceptance;

[TestFixture]
public class CreateCharacterFeature
{
    [Test]
    public async Task Create_a_character_in_the_database_and_respond_with_the_characters_id()
    {
        var repository = new MongoDbCharacterRepository(Fixture.Client);
        var characterService = new CharacterService();
        var action = new CreateCharacter(characterService);
        var api = new Characters(action);
        var frodo = new CreateCharacterCommand("Frodo");
        
        var response = await api.Create(frodo) as OkObjectResult;
        var frodoId = JsonSerializer.Deserialize<CharacterId>(response!.Value!.ToString()!);
        var savedCharacter = await repository.Get(frodoId!);
        
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            Assert.That(frodoId, Is.Not.Null);
            Assert.That(savedCharacter.Name, Is.EqualTo(frodo.Name));
        });
    }
}