using Discount.Shared.Data;
using Discount.Shared.Entities;
using GreatIdeas.Repository;

namespace Discount.Shared.Repositories;

public interface IDiscountRepository : IRepositoryFactory<ApplicationDbContext, Coupon>
{
}
