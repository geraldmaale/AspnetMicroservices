using Microsoft.AspNetCore.Identity;

namespace IdentityServer.IDP;

public class ApplicationUser : IdentityUser
{
    [PersonalData] public string FirstName { get; set; }
    [PersonalData] public string LastName { get; set; }
}