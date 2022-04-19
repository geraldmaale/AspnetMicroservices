using Microsoft.AspNetCore.Identity;

namespace ShoppingMicroservice.ISP;

public class ApplicationUser: IdentityUser
{
    [PersonalData] public string FirstName { get; set; }
    [PersonalData] public string LastName { get; set; }
}