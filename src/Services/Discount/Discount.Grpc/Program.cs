using System.Reflection;
using Discount.Grpc.Mappers;
using Discount.Grpc.Services;
using Discount.Shared.Extensions;
using Discount.Shared.Repositories;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.RegisterDbContextServiceCollection(builder.Configuration, builder.Environment);
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

// Mapster Mapping
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(CouponMappingRegister))!);

var app = builder.Build();

app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();