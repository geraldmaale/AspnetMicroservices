using Discount.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discount.API.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Coupon>? Coupons { get; set; }
}
