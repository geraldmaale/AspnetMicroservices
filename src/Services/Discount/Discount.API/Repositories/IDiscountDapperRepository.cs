using Discount.API.Entities;

namespace Discount.API.Repositories;
public interface IDiscountDapperRepository
{
    Task<Coupon> GetByProductNameAsync(string productName);
    Task<bool> CreateAsync(Coupon coupon);
    Task<bool> UpdateAsync(Coupon coupon);
    Task<bool> DeleteAsync(string productName);
}
