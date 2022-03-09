using Discount.Shared.Extensions;
using Discount.Shared.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.RegisterDbContextServiceCollection(builder.Configuration, builder.Environment);
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

// Mapster Mapping
// TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(ApplicationMappingRegister)));

var app = builder.Build();

// Configure the HTTP request pipeline.
// Run migration
MigrationExtension<Program>.MigrateDatabase(app);

// app.MapGrpcService<>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();
