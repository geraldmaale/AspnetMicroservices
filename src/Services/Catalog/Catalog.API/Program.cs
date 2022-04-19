using System.Text;
using Catalog.API;
using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Catalog.API.Validators;
using Core.Crosscutting;
using Core.Logging;
using FluentValidation.AspNetCore;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
    
    // Add Caching
    builder.Services.AddHttpCacheHeaders((expirationModelOptions) =>
        {
            expirationModelOptions.MaxAge = Constants.TwoMinutesCacheResponse;
            expirationModelOptions.CacheLocation = CacheLocation.Public;
        },
        (validationModelOptions =>
        {
            validationModelOptions.MustRevalidate = true;
        })
    ).AddResponseCaching();

    // Add services to the container.
    builder.Services.AddControllers(actions =>
        {
            // bind cache profiles
            // actions.CacheProfiles.Add(Constants.FiveMinutesCacheProfileResponse, new CacheProfile
            // {
            //     Duration = 300
            // });
        })
        .AddFluentValidation(fv =>
            fv.RegisterValidatorsFromAssemblyContaining<CategoryValidator>());

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    // Access Authentication for IDP
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // "bearer" token
        .AddJwtBearer(options =>
        {
            options.Authority = "https://localhost:5001/";
            options.Audience = ApiResourceConstants.CatalogApi;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = "https://localhost:5001/",
                ValidateAudience = false,
                ValidAudience = ApiResourceConstants.CatalogApi,
                ValidateLifetime = true,
                // ValidateIssuerSigningKey = true,
                ValidTypes = new []{"at+jwt"},
                // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
            };
        });
    
    // Add repositories
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

    // Use Cors
    app.UseCors(policy => policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
    
    // Use caching
    app.UseResponseCaching();
    app.UseHttpCacheHeaders();

    app.UseAuthentication();
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