using Microsoft.Azure.Cosmos;

namespace Infrastructure.Data;

public class CharacterSeeder
{
    public async Task SeedDatabase(CosmosClient client)
    {
        const string databaseName = "drawsteel";
        const string containerName = "characters";
        var database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result.Database;
        var containerProperties = new ContainerProperties
        {
            Id = containerName
        };
        await database.CreateContainerAsync(containerProperties);
    }
}