using Infrastructure.Data;
using Microsoft.Azure.Cosmos;

namespace Tests.Integration.Infrastructure;

[TestFixture]
public class CharacterSeederShould
{
    [Test]
    public async Task Create_Characters_container()
    {
        var connectionString = $"{Fixture.ConnectionString};DisableServerCertificateValidation=True;";
        var clientOptions = new CosmosClientOptions
        {
            HttpClientFactory = () => Fixture.HttpClient,
            ConnectionMode = ConnectionMode.Gateway
        };
        var client = new CosmosClient(connectionString, clientOptions);
        var seeder = new CharacterSeeder(client);
        
        await seeder.SeedDatabase();
        var database = client.GetDatabase("drawsteel");
        
        Assert.That(database.GetContainer("characters"), Is.Not.Null);
    }
}