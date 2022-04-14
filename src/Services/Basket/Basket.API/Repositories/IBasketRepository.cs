using Basket.API.Entities;

namespace Basket.API.Repositories;

public interface IBasketRepository
{
    Task<ShoppingCart?> GetBasketAsync(string? userName);
    Task<ShoppingCart?> UpdateAsync(ShoppingCart basket);
    Task DeleteAsync(string userName);
}