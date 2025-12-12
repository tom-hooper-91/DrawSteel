using Application;
using Azure.Monitor.OpenTelemetry.Exporter;
using Domain;
using Domain.Repositories;
using Infrastructure;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry
const string serviceName = "DrawSteel.API";
var appInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
var useAzureMonitor = !string.IsNullOrEmpty(appInsightsConnectionString);

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    
    if (useAzureMonitor)
        options.AddAzureMonitorLogExporter(o => o.ConnectionString = appInsightsConnectionString);
    else
        options.AddOtlpExporter();
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
        
        if (useAzureMonitor)
            tracing.AddAzureMonitorTraceExporter(o => o.ConnectionString = appInsightsConnectionString);
        else
            tracing.AddOtlpExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
        
        if (useAzureMonitor)
            metrics.AddAzureMonitorMetricExporter(o => o.ConnectionString = appInsightsConnectionString);
        else
            metrics.AddOtlpExporter();
    });

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
app.MapControllers();
// app.UseExceptionHandler("/error");
await app.RunAsync();