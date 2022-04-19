using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShoppingMicroservice.ISP;

public class UserDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }
}