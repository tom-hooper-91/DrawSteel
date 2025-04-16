using System.Data;
using System.Security.Authentication;
using Domain;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class Extensions
{
    public static FunctionsApplicationBuilder ConfigureDatabase(this FunctionsApplicationBuilder builder, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new NoNullAllowedException("MongoDB Connection String is not set");
        }
        
        var settings = MongoClientSettings.FromUrl(
            new MongoUrl(connectionString)
        );
        
        settings.SslSettings = 
            new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        
        var client = new MongoClient(settings);
        
        var collection = client.GetDatabase(DatabaseConstants.DrawSteel)
            .GetCollection<Character>(DatabaseConstants.Characters);
        const string name = "Frodo";
        
        if (!collection.Find(character => character.Name == name).Any())
        {
            collection.InsertOne(new Character(name));
        }
        
        builder.Services.AddSingleton<IMongoClient>(client);
        return builder;
    }
}