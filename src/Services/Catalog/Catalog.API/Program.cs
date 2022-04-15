using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Extensions;
using Catalog.API.Repositories;
using Catalog.API.Validators;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    // Add logging
    builder.AddLoggingServices();

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

    app.UseSerilogRequestLogging(config => {
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