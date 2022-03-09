using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Catalog.API.Validators;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<CategoryValidator>());

// Mapster Mapping
// TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(ApplicationMappingRegister)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

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


app.UseAuthorization();

app.MapControllers();

app.Run();
