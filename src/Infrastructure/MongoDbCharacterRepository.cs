using Domain;
using Domain.Repositories;
using MongoDB.Driver;

namespace Infrastructure;

public class MongoDbCharacterRepository(IMongoClient client) : ICharacterRepository
{
    private IMongoCollection<Character> Characters { get; } = client.GetDatabase(DatabaseConstants.DrawSteel)
        .GetCollection<Character>(DatabaseConstants.Characters);

    public async Task<CharacterId> Add(Character character)
    {
        await Characters.InsertOneAsync(character);
        return character.Id;
    }

    public async Task<Character> Get(CharacterId id)
    {
        return await Characters.Find(character => character.Id == id).SingleOrDefaultAsync();
    }
}