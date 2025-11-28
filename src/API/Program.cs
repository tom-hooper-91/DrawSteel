using API.Diagnostics;
using API.Validation;
using Application;
using Domain;
using Domain.Repositories;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddScoped<ICreateCharacter, CreateCharacter>()
    .AddScoped<IGetCharacter, GetCharacter>()
    .AddScoped<IUpdateCharacter, UpdateCharacter>()
    .AddScoped<IDeleteCharacter, DeleteCharacter>()
    .AddScoped<IListCharacters, ListCharacters>()
    .AddScoped<ICharacterService, CharacterService>()
    .AddScoped<ICharacterRepository, MongoDbCharacterRepository>()
    .AddScoped<ModelStateValidationFilter>();

builder.Services
    .AddControllers(options =>
    {
        options.Filters.AddService<ModelStateValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(state => state.Value?.Errors.Count > 0)
            .ToDictionary(
                pair => pair.Key,
                pair => pair.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        var detail = "Payload validation failed";
        var problem = ValidationProblemFactory.InvalidPayload(detail, errors);
        return problem.ToResult();
    };
});

var connectionString = builder.Configuration["MONGODB_CONNECTION_STRING"] ??
                      builder.Configuration.GetConnectionString("MongoDB");

builder.ConfigureDatabase(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
// app.UseExceptionHandler("/error");
await app.RunAsync();

public partial class Program;