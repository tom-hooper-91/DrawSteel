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

    // T023: Implement Update method using ReplaceOneAsync
    public async Task<bool> Update(Character character)
    {
        var filter = Builders<Character>.Filter.Eq(c => c.Id, character.Id);
        var result = await Characters.ReplaceOneAsync(filter, character);
        return result.MatchedCount > 0;
    }

    public async Task<bool> Delete(CharacterId id)
    {
        var filter = Builders<Character>.Filter.Eq(c => c.Id, id);
        var result = await Characters.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}