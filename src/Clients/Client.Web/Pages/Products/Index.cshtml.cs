using Client.Web.Models;
using Client.Web.Services;
using Core.Crosscutting;
using GreatIdeas.Extensions;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Serilog;

namespace Client.Web.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ITokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ITokenService tokenService, IHttpClientFactory httpClientFactory)
        {
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
        }

        public List<Product> Products { get; set; } = new();
        
        public async Task OnGetAsync()
        {
            try
            {
                var token = _tokenService.GetTokenAsync(ScopeConstants.CatalogApiProduct).Result.AccessToken;
                // var token = await HttpContext.GetTokenAsync("access_token");
                
                var apiClient = _httpClientFactory!.CreateClient("CatalogAPI");
                apiClient.SetBearerToken(token);

                Log.Information("Token: {Token}", token);
            
                var response = await apiClient.GetAsync("api/v1/Products");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<ApiResults<Product>>(content);
                    Products = products!.Results.ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error getting products");
            }
        }
    }
}
