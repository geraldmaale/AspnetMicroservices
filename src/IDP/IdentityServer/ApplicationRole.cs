using Microsoft.AspNetCore.Identity;

namespace IdentityServer;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; }
}