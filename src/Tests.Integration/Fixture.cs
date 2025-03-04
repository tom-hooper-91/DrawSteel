using Testcontainers.CosmosDb;

namespace Tests.Integration;

[SetUpFixture]
public class Fixture
{
    private CosmosDbContainer _cosmosDbContainer;
    public static string ConnectionString { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _cosmosDbContainer = new CosmosDbBuilder()
            .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
            .Build();
        await _cosmosDbContainer.StartAsync();
        ConnectionString = _cosmosDbContainer.GetConnectionString();
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _cosmosDbContainer.DisposeAsync();
    }
}