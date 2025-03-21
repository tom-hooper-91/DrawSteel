using Domain;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class Extensions
{
    public static FunctionsApplicationBuilder ConfigureDatabase(this FunctionsApplicationBuilder builder)
    {
        // var client = new MongoClient(Environment.GetEnvironmentVariable("MongoDB:ConnectionString"));
        var client = new MongoClient("mongodb://localhost:27017");
        // nothing is created exist until a document is inserted
        var database = client.GetDatabase(DatabaseConstants.DrawSteel);
        var collection = database.GetCollection<Character>(DatabaseConstants.Characters);
        collection.InsertOne(new Character("Test")); // this bricks it
        builder.Services.AddSingleton(collection);
        return builder;
    }
}