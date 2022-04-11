using Discount.Shared.Data;
using Discount.Shared.Entities;
using GreatIdeas.Repository;
using Microsoft.EntityFrameworkCore;

namespace Discount.Shared.Repositories;
public class DiscountRepository : RepositoryFactory<DiscountDbContext, Coupon>, IDiscountRepository
{
    public DiscountRepository(IDbContextFactory<DiscountDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
