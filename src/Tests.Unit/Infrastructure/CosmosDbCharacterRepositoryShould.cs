using Domain;
using FakeItEasy;
using Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Tests.Infrastructure;

[TestFixture]
public class CosmosDbCharacterRepositoryShould
{
    [Test]
    public async Task Save_a_Character()
    {
        var character = A.Fake<Character>();
        var characterId = character.Id;
        var container = A.Fake<Container>();
        var response = A.Fake<ItemResponse<Character>>();
        var partitionKey = new PartitionKey("/id");
        var repository = new CosmosDbCharacterRepository(container, partitionKey);
        // A.CallTo(() => response.Resource).Returns(character);
        A.CallTo(
                () => container.CreateItemAsync(character, partitionKey, default, default))
            .Returns(Task.FromResult(response));
        A.CallTo(() => container.ReadItemAsync<Character>(characterId.ToString(), partitionKey, default, default))
            .Returns(Task.FromResult(response));
        
        await repository.Add(character);
        
        Assert.Multiple(async () =>
        {
            Assert.That(await repository.Get(characterId), Is.EqualTo(character));
            A.CallTo(() => container.ReadItemAsync<Character>(characterId.ToString(), new PartitionKey(character.Id.ToString()), default, default))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => container.CreateItemAsync(character, new PartitionKey(character.Id.ToString()), default, default))
                .MustHaveHappenedOnceExactly();
        });
    }
}