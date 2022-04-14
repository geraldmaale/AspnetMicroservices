using EventBus.Messages.Commons;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Data;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderDbContext orderContext,
        ILogger<OrderDbContext> logger)
    {
        try
        {
            if (!orderContext.Orders!.Any())
            {
                orderContext.Orders?.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();

                logger.LogInformation("Seed databases completed ...");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
        }
    }

    private static IEnumerable<Order> GetPreconfiguredOrders()
    {
        return new List<Order>
        {
            new Order(){
                UserName = "test",
                FirstName = "test",
                LastName = "test",
                EmailAddress = "test@email.com",
                AddressLine = "test",
                State = "test",
                Country = "test",
                ZipCode = "test",
                CardName = "test",
                CardNumber = "test",
                Expiration = "test",
                CVV = "test",
                PaymentMethod = PaymentMethod.CreditCard,
            }
        };
    }
}
