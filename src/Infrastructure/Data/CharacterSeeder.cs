using Microsoft.Azure.Cosmos;

namespace Infrastructure.Data;

public class CharacterSeeder(CosmosClient client)
{
    public async Task SeedDatabase()
    {
        const string databaseName = "drawsteel";
        const string containerName = "characters";
        var response = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        var database = response.Database;
        var containerProperties = new ContainerProperties
        {
            Id = containerName,
            PartitionKeyPath = "/id"
        };
        await database.CreateContainerIfNotExistsAsync(containerProperties);
    }
}