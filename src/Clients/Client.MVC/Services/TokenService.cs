using Core.Crosscutting;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Serilog;

namespace Client.Web.Services;

public class TokenService : ITokenService
{
    private readonly IOptions<M2MClientSettings> _identityServerSettings;
    private readonly DiscoveryDocumentResponse _discoveryDocument;
    private readonly HttpClient _httpClient;

    public TokenService(IOptions<M2MClientSettings> identityServerSettings, IHttpClientFactory httpClientFactory)
    {
        _identityServerSettings = identityServerSettings;
        _httpClient = httpClientFactory.CreateClient("ISP");

        _discoveryDocument = _httpClient.GetDiscoveryDocumentAsync(_identityServerSettings.Value.DiscoveryUrl).Result;
        if (_discoveryDocument.IsError)
        {
            Log.Error(_discoveryDocument.Exception,
                "Error while retrieving discovery document: {DiscoveryError}",
                _discoveryDocument.Error);
            throw new Exception("Unable to get discovery document", _discoveryDocument.Exception);
        }
    }

    public async Task<TokenResponse> GetTokenAsync(string scope)
    {
        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = _discoveryDocument.TokenEndpoint,
            ClientId = _identityServerSettings.Value.ClientId,
            ClientSecret = _identityServerSettings.Value.ClientSecret,
            Scope = scope
        });

        if (tokenResponse.IsError)
        {
            Log.Error(_discoveryDocument.Exception,
                "Error while retrieving token: {TokenError}",
                tokenResponse.Error);
            throw new Exception("Unable to get token", tokenResponse.Exception);
        }

        return tokenResponse;
    }
}