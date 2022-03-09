using Dapper;
using Discount.Shared.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Shared.Repositories;
public class DiscountDapperRepository : IDiscountDapperRepository
{
    private readonly NpgsqlConnection _connection;

    public DiscountDapperRepository(IConfiguration configuration)
    {
        var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        using (_connection)
        {
            _connection = connection;
        }
    }

    public async Task<Coupon> GetByProductNameAsync(string productName)
    {
        var coupon = await _connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName=@ProductName", productName);

        if (coupon == null)
        {
            return new Coupon() { ProductName = "No Discount", Amount = 0m, Description = "No Discount Description" };
        }

        return coupon;
    }

    public async Task<bool> CreateAsync(Coupon coupon)
    {
        var affected = await _connection.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

        return affected != 0;
    }

    public async Task<bool> UpdateAsync(Coupon coupon)
    {
        var affected = await _connection.ExecuteAsync("UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

        return affected != 0;
    }

    public async Task<bool> DeleteAsync(string productName)
    {
        var affected = await _connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName=@ProductName", new { ProductName = productName });

        return affected != 0;
    }
}
