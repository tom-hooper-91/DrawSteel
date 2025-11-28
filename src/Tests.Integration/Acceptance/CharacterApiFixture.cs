using System;
using System.Collections.Generic;
using API;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Tests.Integration.Acceptance;

[SetUpFixture]
public sealed class CharacterApiFixture
{
    private MongoDbContainer _mongoDbContainer = null!;
    private WebApplicationFactory<Program> _apiFactory = null!;
    private MongoClient _mongoClient = null!;

    public static HttpClient HttpClient { get; private set; } = null!;
    public static IMongoCollection<Character> Characters { get; private set; } = null!;
    public static MongoClient MongoClient { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task InitializeAsync()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:6.0")
            .Build();

        await _mongoDbContainer.StartAsync();

        _mongoClient = new MongoClient(_mongoDbContainer.GetConnectionString());
        MongoClient = _mongoClient;
        Characters = _mongoClient.GetDatabase(DatabaseConstants.DrawSteel)
            .GetCollection<Character>(DatabaseConstants.Characters);

        Environment.SetEnvironmentVariable("MONGODB_CONNECTION_STRING", _mongoDbContainer.GetConnectionString());

        _apiFactory = new CustomApiFactory(_mongoDbContainer.GetConnectionString());
        HttpClient = _apiFactory.CreateClient();

        await ClearCharactersAsync();
    }

    [OneTimeTearDown]
    public async Task DisposeAsync()
    {
        HttpClient?.Dispose();
        _apiFactory?.Dispose();
        if (_mongoClient is not null)
        {
            _mongoClient.Dispose();
        }

        if (_mongoDbContainer is not null)
        {
            await _mongoDbContainer.DisposeAsync();
        }
    }

    public static Task ClearCharactersAsync() =>
        Characters.DeleteManyAsync(FilterDefinition<Character>.Empty);

    private sealed class CustomApiFactory(string connectionString) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var configurationValues = new Dictionary<string, string?>
            {
                ["MONGODB_CONNECTION_STRING"] = connectionString
            };

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(configurationValues);
            });
        }
    }
}
