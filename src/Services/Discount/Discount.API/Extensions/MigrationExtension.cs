using Discount.API.Data;
using Discount.API.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Discount.API.Extensions;
public static class MigrationExtension
{
    public static void MigrateDatabase(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Migrating database to  Postgres");

        // Migrate EF Core
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var coupons = new List<Coupon>()
        {
            new Coupon() {ProductName = "IPhone X", Description = "IPhone Discount", Amount = 150},
            new Coupon() {ProductName = "Samsung 10", Description = "Samsung Discount", Amount = 120},
        };
        context.Coupons!.AddRange(coupons);
        context.SaveChanges();
        logger.LogInformation("Migrated database to Postgres using EF Core ...");


        // Migrate Dapper
        try
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            connection.Open();

            using var command = new NpgsqlCommand() { Connection = connection };
            command.CommandText = "DROP TABLE IF EXISTS Coupon";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, ProductName VARCHAR(24) NOT NULL, Description TEXT, Amount NUMERIC)";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone x', 'IPhone Discount', 150)";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 120)";
            command.ExecuteNonQuery();

            connection.Close();

            logger.LogInformation("Migrated database to  Postgres using Dapper ...");

        }
        catch (Exception e)
        {
            logger.LogCritical(e, "An error occurred while migration the databases");
        }
    }
}
