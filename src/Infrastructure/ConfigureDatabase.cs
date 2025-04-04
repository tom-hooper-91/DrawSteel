using Domain;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class Extensions
{
    public static FunctionsApplicationBuilder ConfigureDatabase(this FunctionsApplicationBuilder builder)
    {
        var client = new MongoClient("mongodb://root:example@mongodb:27017/");
        var database = client.GetDatabase(DatabaseConstants.DrawSteel);
        var collection = database.GetCollection<Character>(DatabaseConstants.Characters);
        collection.InsertOne(new Character("Test"));
        builder.Services.AddSingleton(collection);
        return builder;
    }
}