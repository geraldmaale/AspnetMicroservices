using Discount.API.Data;
using Discount.API.Entities;
using GreatIdeas.Repository;
using Microsoft.EntityFrameworkCore;

namespace Discount.API.Repositories;
public class DiscountRepository : RepositoryFactory<ApplicationDbContext, Coupon>, IDiscountRepository
{
    public DiscountRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
