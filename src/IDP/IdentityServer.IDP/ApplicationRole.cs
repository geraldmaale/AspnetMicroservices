using Microsoft.AspNetCore.Identity;

namespace IdentityServer.IDP;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; }
}