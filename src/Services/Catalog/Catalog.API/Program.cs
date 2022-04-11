using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Catalog.API.Validators;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web host ...");
    var builder = WebApplication.CreateBuilder(args);

    // Full setup of serilog logging
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((context, services, configuration) => configuration
        // .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
        .WriteTo.Console()
        .WriteTo.Seq(builder.Configuration["SeqConfiguration:Uri"])
        .WriteTo.File("logs/catalogapi-logs.log", rollingInterval: RollingInterval.Day)
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:Uri"]))
        {
            AutoRegisterTemplate = true,
            NumberOfShards = 2,
            IndexFormat =
                $"{builder.Configuration["ApplicationName"]}-logs-{builder.Environment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM-dd}"
        })
        .WriteTo.ApplicationInsights(builder.Services.BuildServiceProvider().GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces)
    );
    
    // Application Insights
    var instrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"];
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.InstrumentationKey = instrumentationKey;
    });

    // Add services to the container.
    builder.Services.AddControllers()
        .AddFluentValidation(fv =>
            fv.RegisterValidatorsFromAssemblyContaining<CategoryValidator>());

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddScoped<ICatalogContext, CatalogContext>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

    builder.Services.AddCors();

    // Seed
    var mongoContext = new CatalogContext(builder.Configuration);
    var productCollection = mongoContext.ConnectToMongo<Product>(CatalogContext.ProductCollection);
    var categoryCollection = mongoContext.ConnectToMongo<Category>(CatalogContext.CategoryCollection);
    CatalogContextSeed.SeedData(productCollection, categoryCollection);

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

    app.UseCors(policy => policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal error occurred");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;