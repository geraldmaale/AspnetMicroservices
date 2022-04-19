using Microsoft.AspNetCore.Identity;

namespace ShoppingMicroservice.ISP;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; }
}