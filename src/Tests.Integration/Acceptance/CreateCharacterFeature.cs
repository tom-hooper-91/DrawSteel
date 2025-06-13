using System.Text;
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
        var factory = new CharacterFactory();
        var repository = new MongoDbCharacterRepository(Fixture.Client);
        var action = new CreateCharacter(factory, new SaveCharacter(repository));
        var api = new Characters(action);
        var character = new CreateCharacterCommand("Frodo");
        
        var response = await api.Create(character) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(response!.Value!.ToString()!);
        var characterFromDatabase = await repository.Get(characterId!);
        
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            Assert.That(characterId, Is.Not.Null);
            Assert.That(characterFromDatabase.Name, Is.EqualTo(character.Name));
        });
    }
}