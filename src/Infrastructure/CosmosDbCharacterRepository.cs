using Domain;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;

namespace Infrastructure;

public class CosmosDbCharacterRepository(Container container, PartitionKey partitionKey) : ICharacterRepository
{
    public async Task<CharacterId> Add(Character character)
    {
        var response = await container.CreateItemAsync(character);
        return response.Resource.Id;
    }

    public async Task<Character> Get(CharacterId id)
    {
        var response = await container.ReadItemAsync<Character>(id.ToString(), partitionKey, default, default);
        return response.Resource;
    }
}