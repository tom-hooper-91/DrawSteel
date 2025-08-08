using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Tests.Integration;

[SetUpFixture]
public class Fixture
{
    private MongoDbContainer _mongoDbContainer;
    public static MongoClient Client = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:6.0")
            .WithName("test-containers-mongodb")
            .Build();
        await _mongoDbContainer.StartAsync();
        Client = new MongoClient(_mongoDbContainer.GetConnectionString());
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _mongoDbContainer.DisposeAsync();
        Client.Dispose();
    }
}