using System.Data;
using System.Security.Authentication;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class Extensions
{
    public static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new NoNullAllowedException("MongoDB Connection String is not set");
        }
        
        // var settings = MongoClientSettings.FromUrl(
        //     new MongoUrl(connectionString)
        // );
        //
        // settings.SslSettings = 
        //     new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        
        var client = new MongoClient(connectionString);
        
        var collection = client.GetDatabase(DatabaseConstants.DrawSteel)
            .GetCollection<Character>(DatabaseConstants.Characters);
        const string frodo = "Frodo";
        
        if (!collection.Find(character => character.Name == frodo).Any())
        {
            collection.InsertOne(new Character(frodo));
        }
        
        builder.Services.AddSingleton<IMongoClient>(client);
        return builder;
    }
}