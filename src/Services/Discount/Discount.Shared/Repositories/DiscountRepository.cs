using Discount.Shared.Data;
using Discount.Shared.Entities;
using GreatIdeas.Repository;
using Microsoft.EntityFrameworkCore;

namespace Discount.Shared.Repositories;
public class DiscountRepository : RepositoryFactory<ApplicationDbContext, Coupon>, IDiscountRepository
{
    public DiscountRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
