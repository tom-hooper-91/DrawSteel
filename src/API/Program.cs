using Application;
using Domain;
using Domain.Repositories;
using Infrastructure;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddScoped<ICreateCharacter, CreateCharacter>()
    .AddScoped<ICharacterFactory, CharacterFactory>()
    .AddScoped<ISaveCharacter, SaveCharacter>()
    .AddScoped<ICharacterRepository, MongoDbCharacterRepository>();

// This code successfully pulls the connection string from local.settings.json
// but it completely breaks the function builder for some reason:
// System.InvalidOperationException: Configuration is missing the 'HostEndpoint' information.
// Please ensure an entry with the key 'Functions:Worker:HostEndpoint' is present in your configuration.

// var configuration = new ConfigurationBuilder()
//     .SetBasePath(Directory.GetCurrentDirectory())
//     .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
//     .AddEnvironmentVariables()
//     .Build();
//
// builder.ConfigureDatabase(configuration.GetConnectionString("MongoDB") ??
//                             Environment.GetEnvironmentVariable("MongoDBConnectionString"));

builder.ConfigureDatabase("mongodb://root:example@mongodb:27017/");

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

await builder.Build().RunAsync();