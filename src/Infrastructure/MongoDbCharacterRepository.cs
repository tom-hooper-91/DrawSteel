using Domain;
using Domain.Repositories;
using MongoDB.Driver;

namespace Infrastructure;

public class MongoDbCharacterRepository(MongoClient client) : ICharacterRepository
{
    public async Task<CharacterId> Add(Character character)
    {
        var characters = client.GetDatabase(DatabaseConstants.DrawSteel)
        .GetCollection<Character>(DatabaseConstants.Characters);
        await characters.InsertOneAsync(character);
        return character.Id;
    }

    public async Task<Character> Get(CharacterId id)
    {
        var characters = client.GetDatabase(DatabaseConstants.DrawSteel)
            .GetCollection<Character>(DatabaseConstants.Characters);
        return await characters.Find(character => character.Id == id).SingleOrDefaultAsync();
    }
}