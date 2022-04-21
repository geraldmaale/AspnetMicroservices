using Client.Wasm.Models;
using GreatIdeas.Extensions;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Client.Wasm.Pages;

public partial class Products
{
    [Inject] public IHttpClientFactory? HttpClientFactory { get; set; }
    protected List<Product> ProductList { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {
        await GetData();
    }

    private async Task GetData()
    {
        try
        {
            var apiClient = HttpClientFactory!.CreateClient("CatalogAPI");

            var response = await apiClient.GetAsync("api/v1/products");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResults<Product>>(content);
                ProductList = result!.Results.ToList();
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
