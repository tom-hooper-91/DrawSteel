using Application;
using Domain;
using Domain.Repositories;
using Infrastructure;

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
    .AddScoped<ICharacterService, CharacterService>()
    .AddScoped<ICharacterRepository, MongoDbCharacterRepository>()
    .AddControllers();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.ConfigureDatabase(configuration.GetConnectionString("MongoDB") ??
                          configuration["MONGODB_CONNECTION_STRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers().WithOpenApi();
// app.UseExceptionHandler("/error");
await app.RunAsync();