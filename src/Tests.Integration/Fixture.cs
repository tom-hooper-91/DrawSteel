using Microsoft.Azure.Cosmos;
using Testcontainers.CosmosDb;

namespace Tests.Integration;

[SetUpFixture]
public class Fixture
{
    private CosmosDbContainer _cosmosDbContainer;
    public static CosmosClient Client { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _cosmosDbContainer = new CosmosDbBuilder()
            .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
            .WithName("integration-test-cosmosdb")
            .Build();
        await _cosmosDbContainer.StartAsync();
        var connectionString = $"{_cosmosDbContainer.GetConnectionString()};DisableServerCertificateValidation=True;";
        var clientOptions = new CosmosClientOptions
        {
            HttpClientFactory = () => _cosmosDbContainer.HttpClient,
            ConnectionMode = ConnectionMode.Gateway
        };
        Client = new CosmosClient(connectionString, clientOptions);
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _cosmosDbContainer.DisposeAsync();
        Client.Dispose();
    }
}