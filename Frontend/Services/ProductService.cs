using System.Net.Http.Json;
using ProductCrudBlazor.Models;
using Microsoft.Extensions.Configuration;

namespace ProductCrudBlazor.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ProductService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:ProductApiUrl"] 
                ?? throw new InvalidOperationException("ProductApiUrl no configurado");
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/products");
            return await response.Content.ReadFromJsonAsync<List<Product>>();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Product>($"{_apiBaseUrl}/api/products/{id}");
        }

        public async Task CreateProductAsync(Product product)
        {
            await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/products", product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/products/{product.Id}", product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/products/{id}");
        }
    }
}
