using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddLoggingServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .RegisterDbContextService(builder.Configuration, builder.Environment)
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

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

// Add migrations
MigrationExtension<OrderDbContext>.MigrateDatabase(app);

app.UseAuthorization();

app.MapControllers();

app.Run();
