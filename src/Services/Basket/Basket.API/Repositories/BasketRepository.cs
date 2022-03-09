using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _distributedCache;

    public BasketRepository(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    }

    public async Task<ShoppingCart?> GetAsync(string? userName)
    {
        var basket = await _distributedCache.GetStringAsync(userName);
        if (string.IsNullOrEmpty(basket))
            return null;

        return JsonConvert.DeserializeObject<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart?> UpdateAsync(ShoppingCart basket)
    {
        await _distributedCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
        return await GetAsync(basket.UserName);
    }

    public async Task DeleteAsync(string userName)
    {
        await _distributedCache.RemoveAsync(userName);

    }
}