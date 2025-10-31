using Domain;
using Domain.Repositories;
using Infrastructure;
using MongoDB.Driver;
using FakeItEasy;

namespace Tests.Infrastructure;

[TestFixture]
public class MongoDbCharacterRepositoryShould
{
#pragma warning disable NUnit1032
    private IMongoClient _client;
#pragma warning restore NUnit1032
    private IMongoDatabase _database;
    private IMongoCollection<Character> _collection;
    private MongoDbCharacterRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _client = A.Fake<IMongoClient>();
        _database = A.Fake<IMongoDatabase>();
        _collection = A.Fake<IMongoCollection<Character>>();
        
        A.CallTo(() => _client.GetDatabase(DatabaseConstants.DrawSteel, null))
            .Returns(_database);
        A.CallTo(() => _database.GetCollection<Character>(DatabaseConstants.Characters, null))
            .Returns(_collection);
        
        _repository = new MongoDbCharacterRepository(_client);
    }

    [Test]
    public async Task Update_returns_true_when_character_found()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var character = new Character(characterId, "Updated Name");
        var replaceResult = A.Fake<ReplaceOneResult>();
        A.CallTo(() => replaceResult.MatchedCount).Returns(1);
        A.CallTo(() => _collection.ReplaceOneAsync(
            A<FilterDefinition<Character>>._, 
            character, 
            A<ReplaceOptions>._, 
            default))
            .Returns(replaceResult);

        var result = await _repository.Update(character);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Update_returns_false_when_character_not_found()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var character = new Character(characterId, "Updated Name");
        var replaceResult = A.Fake<ReplaceOneResult>();
        A.CallTo(() => replaceResult.MatchedCount).Returns(0);
        A.CallTo(() => _collection.ReplaceOneAsync(
            A<FilterDefinition<Character>>._, 
            character, 
            A<ReplaceOptions>._, 
            default))
            .Returns(replaceResult);

        var result = await _repository.Update(character);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Delete_returns_true_when_character_deleted()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var deleteResult = A.Fake<DeleteResult>();
        A.CallTo(() => deleteResult.DeletedCount).Returns(1);
        A.CallTo(() => _collection.DeleteOneAsync(
            A<FilterDefinition<Character>>._, 
            default))
            .Returns(deleteResult);

        var result = await _repository.Delete(characterId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Delete_returns_false_when_already_deleted()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var deleteResult = A.Fake<DeleteResult>();
        A.CallTo(() => deleteResult.DeletedCount).Returns(0);
        A.CallTo(() => _collection.DeleteOneAsync(
            A<FilterDefinition<Character>>._, 
            default))
            .Returns(deleteResult);

        var result = await _repository.Delete(characterId);

        Assert.That(result, Is.False);
    }
}
