using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Client.MVC.Models;
using Client.Web.Models;
using Client.Web.Services;
using Core.Crosscutting;
using GreatIdeas.Extensions;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Serilog;

namespace Client.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITokenService _tokenService;
    private readonly IHttpClientFactory _httpClientFactory;
    

    public HomeController(ILogger<HomeController> logger, ITokenService tokenService, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _tokenService = tokenService;
        _httpClientFactory = httpClientFactory;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> Products()
    {
        try
        {
            // var token = _tokenService.GetTokenAsync(ScopeConstants.CatalogApiProduct).Result.AccessToken;
            var token = await HttpContext.GetTokenAsync("access_token");
            var apiClient = _httpClientFactory!.CreateClient("CatalogAPI");
            apiClient.SetBearerToken(token);

            Log.Information("Token: {Token}", token);
            
            var response = await apiClient.GetAsync("api/v1/Products");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<ApiResults<Product>>(content);
                return View(products!.Results.ToList());
            }
            
            return View();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting products");
            return BadRequest();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}