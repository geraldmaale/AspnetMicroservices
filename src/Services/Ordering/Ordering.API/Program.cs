using System.Reflection;
using Core.Logging;
using EventBus.Messages.Events;
using Mapster;
using Ordering.API.Extensions;
using Ordering.API.Middlewares;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data;
using Serilog;

// Serilog Boostrap
LoggingServiceCollection.AddSerilogBootstrapLogging();

try
{
    var builder = WebApplication.CreateBuilder(args);

// Add logging services
    builder.AddLoggingServices();
    builder.Services.AddTransient<ExceptionHandlingMiddleware>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services
        .AddDbContextService(builder.Configuration, builder.Environment)
        .AddInfrastructureServices(builder.Configuration)
        .AddApplicationServices();
    builder.AddMassTransitWithConsumerServices();

// Mapster Mapping
    TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

    builder.Services.AddScoped<BasketCheckoutEvent>();

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging(config =>
    {
        config.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} {UserId} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

// Use logging service middleware
    app.UseSerilogCustomLoggingMiddleware();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

// Add migrations
    MigrationExtension<OrderDbContext>.MigrateDatabase(app);

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException")
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}