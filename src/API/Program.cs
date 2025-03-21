using Application;
using Domain;
using Domain.Repositories;
using Infrastructure;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddScoped<ICreateCharacter, CreateCharacter>();
builder.Services.AddScoped<ICharacterFactory, CharacterFactory>();
builder.Services.AddScoped<ISaveCharacter, SaveCharacter>();
builder.Services.AddScoped<ICharacterRepository, MongoDbCharacterRepository>();
builder.ConfigureDatabase();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

await builder.Build().RunAsync();