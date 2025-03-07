using Domain;
using FakeItEasy;
using Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Tests.Infrastructure;

[TestFixture]
public class CosmosDbCharacterRepositoryShould
{
    private Container _container;
    private CosmosDbCharacterRepository _repository;
    private CharacterId _expectedCharacterId;
    private PartitionKey _partitionKey;
    private Character _character;

    [SetUp]
    public void SetUp()
    {
        _container = A.Fake<Container>();
        var response = A.Fake<ItemResponse<Character>>();
        _character = response.Resource;
        _expectedCharacterId = _character.Id;
        _partitionKey = new PartitionKey(_expectedCharacterId.ToString());
        _repository = new CosmosDbCharacterRepository(_container);
        A.CallTo(
                () => _container.CreateItemAsync(_character, _partitionKey, default, default))
            .Returns(Task.FromResult(response));
        A.CallTo(() => _container.ReadItemAsync<Character>(_expectedCharacterId.ToString(), _partitionKey, default, default))
            .Returns(Task.FromResult(response));
    }
    
    [Test]
    public async Task Add_then_Get_a_Character()
    {
        var characterId = await _repository.Add(_character);
        var retrievedCharacter = await _repository.Get(characterId);
        
        Assert.Multiple(() =>
        {
            Assert.That(retrievedCharacter.Id, Is.EqualTo(_expectedCharacterId));
            A.CallTo(() => _container.ReadItemAsync<Character>(_expectedCharacterId.ToString(), _partitionKey, default, default))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _container.CreateItemAsync(_character, _partitionKey, default, default))
                .MustHaveHappenedOnceExactly();
        });
    }
}