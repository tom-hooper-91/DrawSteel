using Domain;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;

namespace Infrastructure;

public class CosmosDbCharacterRepository(Container container) : ICharacterRepository
{
    public async Task<CharacterId> Add(Character character)
    {
        var response = await container.CreateItemAsync(character, new PartitionKey(character.Id.ToString()));
        return response.Resource.Id;
    }

    public async Task<Character> Get(CharacterId id)
    {
        var idString = id.ToString();
        var response = await container.ReadItemAsync<Character>(idString, new PartitionKey(idString));
        return response.Resource;
    }
}