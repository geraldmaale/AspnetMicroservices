using System.Reflection;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Core.Logging;
using Discount.Grpc.Protos;
using Mapster;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add logging services
builder.AddLoggingServices();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis cache
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
});

// Add repos
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// Mapster Mapping
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

// Grpc service services
var grpcUrl = builder.Configuration["GrpcSettings:DiscountUrl"];
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
    options.Address = new Uri(grpcUrl));

builder.Services.AddScoped<DiscountGrpcService>();

// MassTransit-RabbitMQ Config
builder.Services.AddMassTransit(config => {
    config.UsingRabbitMq((context, cfg) => {
        var hostName = builder.Configuration["RabbitMq:HostName"];
        var userName = builder.Configuration["RabbitMq:UserName"];
        var password = builder.Configuration["RabbitMq:Password"];
        var port = builder.Configuration.GetValue<ushort>("RabbitMq:Port");
        
        cfg.Host(host:hostName, port:port,"/" , hostConfig => {
            hostConfig.Username(userName);
            hostConfig.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

// App
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use logging service middleware
app.UseLoggingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
