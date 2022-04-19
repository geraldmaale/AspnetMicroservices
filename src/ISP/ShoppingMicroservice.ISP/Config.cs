using Core.Crosscutting;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace ShoppingMicroservice.ISP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
            new IdentityResource
            {
                Name = "roles",
                DisplayName = "Roles",
                UserClaims = {JwtClaimTypes.Role}
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(ScopeConstants.CatalogApiCategory),
            new ApiScope(ScopeConstants.CatalogApiProduct),
            new ApiScope(ScopeConstants.DiscountApiCoupon),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource(
                ApiResourceConstants.CatalogApi,
                "Catalog API Resource",
                new[] {JwtClaimTypes.Role})
            {
                ApiSecrets = {new Secret("547SC7A1-0D79-1F89-A3D8-A37998FB86B0".Sha256())},
                Scopes = new List<string>() {ScopeConstants.CatalogApiCategory, ScopeConstants.CatalogApiProduct},
                UserClaims = {JwtClaimTypes.Role}
            },
            new ApiResource(
                ApiResourceConstants.DiscountApi,
                "Discount API Resource",
                new[] {JwtClaimTypes.Role})
            {
                ApiSecrets = {new Secret("49a02ea1-d62b-406b-bb56-1e50109aa0b4".Sha256())},
                Scopes = new List<string>() {ScopeConstants.DiscountApiCoupon},
                UserClaims = {JwtClaimTypes.Role}
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = {new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())},

                AllowedScopes = {ScopeConstants.CatalogApiCategory}
            },

            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "catalog-client-blazor",
                ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = {"https://localhost:7130/signin-oidc"},
                FrontChannelLogoutUri = "https://localhost:7130/signout-oidc",
                PostLogoutRedirectUris = {"https://localhost:7130/signout-callback-oidc"},

                AllowOfflineAccess = true,
                RequireConsent = true,
                AllowedScopes =
                {
                    "openid", "profile", "email", ScopeConstants.CatalogApiCategory, ScopeConstants.CatalogApiProduct,
                    ScopeConstants.DiscountApiCoupon
                }
            },
        };
}