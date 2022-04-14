using EventBus.Messages.Commons;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Data;

namespace Ordering.API.Extensions;
public static class MigrationExtension<TContext>
{
    public static void MigrateDatabase(IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();

        logger.LogInformation("Migrating database to Postgres ...");

        // Migrate EF Core
        using var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        context.Database.Migrate();

        if (!context.Orders!.Any())
        {
            logger.LogInformation("Seeding into database ...");

            // create dummy data
            var order = new Order() {
                FirstName = "Test",
                LastName = "User",
                UserName = "gematt",
                EmailAddress = "test@email.com",
                AddressLine = "123 Main Street",
                State = "WA",
                ZipCode = "98101",
                Country = "USA",
                CardNumber = "1234567890123456",
                CardName = "Test User",
                Expiration = "12/20",
                CVV = "123",
                TotalPrice = 150.00m,
                PaymentMethod = PaymentMethod.CreditCard,
                CreatedBy = "gematt",
                CreatedDate = DateTime.UtcNow,
            };
            context.Orders!.Add(order);
            context.SaveChanges();
        }
    }
}
