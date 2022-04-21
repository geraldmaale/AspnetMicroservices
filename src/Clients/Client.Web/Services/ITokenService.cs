using IdentityModel.Client;

namespace Client.Web.Services;

public interface ITokenService
{
    Task<TokenResponse> GetTokenAsync(string scope);
}