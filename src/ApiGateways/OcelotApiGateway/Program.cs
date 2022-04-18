using Core.Logging;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add appsetting configuration
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("ocelot.json", true, true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables()
    .Build();
builder.WebHost.UseConfiguration(config);

// Add Logging
builder.AddLoggingServices();

// Add Ocelot
builder.Services
    .AddOcelot()
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
// Use logging services
app.UseSerilogCustomLoggingMiddleware();

// Use Ocelot
app.UseOcelot().Wait();

app.MapGet("/", () => "Running Ocelot Gateway!!!");

app.Run();