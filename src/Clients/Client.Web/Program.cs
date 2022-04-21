using System.Net.Http.Headers;
using Client.Web.Services;
using Core.Crosscutting;
using Core.Logging;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

// Boostrap logger
LoggingServiceCollection.AddSerilogBootstrapLogging();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.AddLoggingServices();

// Configure Httpclient
builder.Services.AddScoped<HttpClient>();
builder.Services.AddHttpClient("CatalogAPI", c =>
{
    c.BaseAddress = new Uri("http://localhost:8000/");
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

builder.Services.Configure<M2MClientSettings>(builder.Configuration.GetSection(nameof(M2MClientSettings)));
var interactiveClientSettings = builder.Configuration.GetSection(nameof(InteractiveClientSettings)).Get<InteractiveClientSettings>();

builder.Services.AddSingleton<ITokenService, TokenService>();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = interactiveClientSettings.Authority;
        options.ClientId = interactiveClientSettings.ClientId;
        options.ClientSecret = interactiveClientSettings.ClientSecret;
        options.RequireHttpsMetadata = false;

        options.Scope.Clear();
        options.Scope.Add(interactiveClientSettings.Scopes![0]);
        options.Scope.Add(interactiveClientSettings.Scopes![1]);
        options.Scope.Add(interactiveClientSettings.Scopes![2]);
        options.Scope.Add(interactiveClientSettings.Scopes![3]);

        options.ClaimActions.DeleteClaim("sid");
        options.ClaimActions.DeleteClaim("idp");
        options.ClaimActions.DeleteClaim("s-hash");
        options.ClaimActions.DeleteClaim("auth_time");

        options.ClaimActions.MapUniqueJsonKey("role", "role");

        options.ResponseType = OpenIdConnectResponseType.Code;
        options.UsePkce = true;
        options.SaveTokens = true;
        options.ResponseMode = OpenIdConnectResponseMode.Query;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtClaimTypes.GivenName,
            RoleClaimType = JwtClaimTypes.Role,
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSerilogCustomLoggingMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages()
    .RequireAuthorization();

app.Run();
