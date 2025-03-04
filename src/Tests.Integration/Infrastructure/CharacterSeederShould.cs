using Infrastructure.Data;
using Microsoft.Azure.Cosmos;

namespace Tests.Integration.Infrastructure;

[TestFixture]
public class CharacterSeederShould
{
    [Test]
    public async Task Create_Characters_container()
    {
        // var clientOptions = new CosmosClientOptions
        // {
        //     HttpClientFactory = () => new HttpClient(new HttpClientHandler
        //     {
        //         ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        //     }),
        //     ConnectionMode = ConnectionMode.Gateway
        // };
        var client = new CosmosClient(Fixture.ConnectionString);
        var database = client.GetDatabase("drawsteel");
        var seeder = new CharacterSeeder();
        
        await seeder.SeedDatabase(client);
        
        Assert.That(database.GetContainer("characters"), Is.Not.Null);
    }
}