using System.IdentityModel.Tokens.Jwt;
using Client.Catalog.Services;
using Core.Crosscutting;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Clear jwt mapping
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddHttpContextAccessor();

// Configure Httpclient
builder.Services.AddHttpClient();

// Add Token
builder.Services.Configure<M2MClientSettings>(builder.Configuration.GetSection(nameof(M2MClientSettings)));
var interactiveClientSettings = builder.Configuration.GetSection(nameof(InteractiveClientSettings)).Get<InteractiveClientSettings>();

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = interactiveClientSettings.Authority;
        options.ClientId = interactiveClientSettings.ClientId;
        options.ClientSecret = interactiveClientSettings.ClientSecret;

        options.Scope.Add(interactiveClientSettings.Scopes![0]);
        options.Scope.Add(interactiveClientSettings.Scopes![1]);
        options.Scope.Add(interactiveClientSettings.Scopes![2]);
        options.Scope.Add(interactiveClientSettings.Scopes![3]);

        options.ClaimActions.DeleteClaim("sid");
        options.ClaimActions.DeleteClaim("idp");
        options.ClaimActions.DeleteClaim("s-hash");
        options.ClaimActions.DeleteClaim("auth_time");

        options.ClaimActions.MapUniqueJsonKey("role", "role");

        options.ResponseType = OpenIdConnectParameterNames.Code; // "code id_token";
        options.UsePkce = true;
        options.SaveTokens = true;
        options.ResponseMode = "query";
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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();