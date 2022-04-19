using IdentityModel.Client;

namespace Client.Catalog.Services;

public interface ITokenService
{
    Task<TokenResponse> GetTokenAsync(string scope);
}