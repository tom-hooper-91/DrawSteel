using Application;
using Domain;
using Domain.Repositories;
using Infrastructure;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddScoped<ICreateCharacter, CreateCharacter>()
    .AddScoped<ICharacterFactory, CharacterFactory>()
    .AddScoped<ISaveCharacter, SaveCharacter>()
    .AddScoped<ICharacterRepository, MongoDbCharacterRepository>();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.ConfigureDatabase(configuration.GetConnectionString("MongoDB") ??
                          configuration["MONGODB_CONNECTION_STRING"]);

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

await builder.Build().RunAsync();