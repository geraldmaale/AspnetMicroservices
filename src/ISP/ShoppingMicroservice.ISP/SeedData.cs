using System.Security.Claims;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ShoppingMicroservice.ISP;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        scope.ServiceProvider.GetService<PersistedGrantDbContext>()?.Database.Migrate();

        var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
        context?.Database.Migrate();
        EnsureSeedData(context);
        EnsureUsersSeedData(app);
    }

    private static void EnsureSeedData(ConfigurationDbContext context)
    {
        if (!context.Clients.Any())
        {
            Log.Debug("Clients being populated");
            foreach (var client in Config.Clients.ToList())
            {
                context.Clients.Add(client.ToEntity());
            }

            context.SaveChanges();
        }
        else
        {
            Log.Debug("Clients already populated");
        }

        if (!context.IdentityResources.Any())
        {
            Log.Debug("IdentityResources being populated");
            foreach (var resource in Config.IdentityResources.ToList())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }
        else
        {
            Log.Debug("IdentityResources already populated");
        }

        if (!context.ApiScopes.Any())
        {
            Log.Debug("ApiScopes being populated");
            foreach (var resource in Config.ApiScopes.ToList())
            {
                context.ApiScopes.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }
        else
        {
            Log.Debug("ApiScopes already populated");
        }

        if (!context.IdentityProviders.Any())
        {
            Log.Debug("OIDC IdentityProviders being populated");
            context.IdentityProviders.Add(new OidcProvider
            {
                Scheme = "demoidsrv",
                DisplayName = "IdentityServer",
                Authority = "https://demo.duendesoftware.com",
                ClientId = "login",
            }.ToEntity());
            context.SaveChanges();
        }
        else
        {
            Log.Debug("OIDC IdentityProviders already populated");
        }
    }

    private static void EnsureUsersSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<UserDbContext>();
        context?.Database.Migrate();

        Log.Debug("Identity Users and claims being populated");

        #region Insert Default Roles

        // add default roles to db
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // Admin Role
        if (!context!.Roles.Any())
        {
            var adminRole = roleManager.FindByNameAsync("Administrator").Result;
            if (adminRole == null)
            {
                var admin = new ApplicationRole
                    {Name = "Administrator", Description = "Can oversee all activities in the system"};
                var result = roleManager.CreateAsync(admin).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            // User Role
            var userRole = roleManager.FindByNameAsync("Read Only").Result;
            if (userRole == null)
            {
                var user = new ApplicationRole
                    {Name = "Read Only", Description = "Can only view and interact with the system"};
                var result = roleManager.CreateAsync(user).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }
        else
        {
            Log.Debug("Identity Roles already populated");
        }

        #endregion

        // Create test users
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();
            
        if (!context!.Users.Any())
        {
            var adminUser = userManager.FindByNameAsync("Admin").Result;
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "Admin",
                    FirstName = "Simon",
                    LastName = "Moses",
                    EmailConfirmed = true,
                    Email = "admin@email.com",
                    PhoneNumber = "123456789"
                };

                var result = userManager.CreateAsync(adminUser, "P@ssword1").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userManager.AddClaimsAsync(adminUser, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, $"{adminUser.FirstName} {adminUser.LastName}"),
                    new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                    new Claim(JwtClaimTypes.Id, adminUser.Id),
                    new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                    new Claim(JwtClaimTypes.Email, adminUser.Email),
                    new Claim(JwtClaimTypes.PhoneNumber, adminUser.PhoneNumber),
                    new Claim(JwtClaimTypes.PreferredUserName, adminUser.UserName),
                    new Claim(JwtClaimTypes.Role, "Administrator")
                }).Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            // user 2
            var user2 = userManager.FindByNameAsync("User").Result;
            if (user2 == null)
            {
                user2 = new ApplicationUser
                {
                    UserName = "user",
                    FirstName = "Mary",
                    LastName = "Jones",
                    EmailConfirmed = true,
                    Email = "user@email.com",
                    PhoneNumber = "133456789"
                };

                var result = userManager.CreateAsync(user2, "P@ssword1").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userManager.AddClaimsAsync(user2, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, $"{user2.FirstName} {user2.LastName}"),
                    new Claim(JwtClaimTypes.GivenName, user2.FirstName),
                    new Claim(JwtClaimTypes.Id, user2.Id),
                    new Claim(JwtClaimTypes.FamilyName, user2.LastName),
                    new Claim(JwtClaimTypes.Email, user2.Email),
                    new Claim(JwtClaimTypes.PhoneNumber, user2.PhoneNumber),
                    new Claim(JwtClaimTypes.PreferredUserName, user2.UserName),
                    new Claim(JwtClaimTypes.Role, "Read Only")
                }).Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }
        else
        {
            Log.Debug("Identity Users already populated");
        }
    }
}