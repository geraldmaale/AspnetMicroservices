using Discount.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discount.Shared.Data;
public class DiscountDbContext : DbContext
{
    public DiscountDbContext(DbContextOptions<DiscountDbContext> options)
        : base(options)
    {
    }

    public DbSet<Coupon>? Coupons { get; set; }
}
