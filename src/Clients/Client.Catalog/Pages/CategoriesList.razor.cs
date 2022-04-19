using Client.Catalog.Models;
using Client.Catalog.Services;
using Core.Crosscutting;
using GreatIdeas.Extensions;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;


namespace Client.Catalog.Pages
{
    public partial class CategoriesList
    {
        [Inject] public ITokenService? TokenService { get; set; }
        [Inject] public IHttpClientFactory? HttpClientFactory { get; set; }
        protected List<CategoryDto> Categories { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            await GetData();
        }

        private async Task GetData()
        {
            try
            {
                // var token = await TokenService!.GetTokenAsync(ScopeConstants.CatalogApiCategory);
                var client = HttpClientFactory!.CreateClient();
                var token = await TokenService!.GetTokenAsync(ScopeConstants.CatalogApiCategory);
                client.SetBearerToken(token.AccessToken);
                
                var response = await client.GetAsync("http://localhost:8000/api/v1/category");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResults<CategoryDto>>(content);
                    // var result = await JsonSerializer.DeserializeAsync<ApiResults<CategoryDto>>(content);
                    Categories = result!.Results.ToList();
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
        }
    }
}