using Discount.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discount.Shared.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Coupon>? Coupons { get; set; }
}
