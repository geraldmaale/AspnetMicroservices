using Discount.API.Extensions;
using Discount.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterDbContextServiceCollection(builder.Configuration, builder.Environment);
builder.Services.AddScoped<IDiscountDapperRepository, DiscountDapperRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Run migration
app.MigrateDatabase();


app.UseAuthorization();

app.MapControllers();

app.Run();
