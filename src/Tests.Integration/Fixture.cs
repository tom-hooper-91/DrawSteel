using Testcontainers.CosmosDb;

namespace Tests.Integration;

[SetUpFixture]
public class Fixture
{
    private CosmosDbContainer _cosmosDbContainer;
    public static string ConnectionString { get; private set; } = null!;
    public static HttpClient HttpClient { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _cosmosDbContainer = new CosmosDbBuilder()
            .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
            .WithName("integration-cosmosdb")
            .WithPortBinding(8081, 8081)
            .WithPortBinding(10250, 10250)
            .WithPortBinding(10251, 10251)
            .WithPortBinding(10252, 10252)
            .WithPortBinding(10253, 10253)
            .WithPortBinding(10254, 10254)
            .WithPortBinding(10255, 10255)
            .Build();
        await _cosmosDbContainer.StartAsync();
        ConnectionString = _cosmosDbContainer.GetConnectionString();
        HttpClient = _cosmosDbContainer.HttpClient;
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _cosmosDbContainer.DisposeAsync();
    }
}