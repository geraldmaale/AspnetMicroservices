using System.Net.Http.Headers;
using Client.Wasm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Clear jwt mapping
// JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
//JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddOidcAuthentication(options =>
{
    // options.ProviderOptions.Authority = "https://localhost:5001/";
    // options.ProviderOptions.ClientId = "catalog-client-wasm";
    // options.ProviderOptions.RedirectUri = "https://localhost:7040/authentication/login-callback";
    // options.ProviderOptions.PostLogoutRedirectUri = "https://localhost:7040/authentication/login-callback";
    // options.ProviderOptions.DefaultScopes.Add("email");
    // options.ProviderOptions.ResponseType = "code";
    
    builder.Configuration.Bind("OidcConfiguration", options.ProviderOptions);
});

// Configure Httpclient
builder.Services.AddScoped<HttpClient>();
builder.Services.AddHttpClient("CatalogAPI", c =>
{
    c.BaseAddress = new Uri("https://localhost:7196/");
    c.Timeout = new TimeSpan(0, 0, 30);
    c.DefaultRequestHeaders.Clear();
    c.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});
// create an HttpClient used for accessing the IDP
builder.Services.AddHttpClient("ISP", c =>
{
    c.BaseAddress = new Uri("https://localhost:5001/");
    c.Timeout = new TimeSpan(0, 0, 30);
    c.DefaultRequestHeaders.Clear();
    c.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();