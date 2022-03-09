using Discount.API.Data;
using Discount.API.Entities;
using GreatIdeas.Repository;

namespace Discount.API.Repositories;

public interface IDiscountRepository : IRepositoryFactory<ApplicationDbContext, Coupon>
{
}
