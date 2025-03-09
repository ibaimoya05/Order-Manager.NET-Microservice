using ProductCrudBlazor.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace ProductCrudBlazor.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _orderApiUrl;

        public OrderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _orderApiUrl = configuration["ApiSettings:OrderApiUrl"] 
                ?? throw new InvalidOperationException("OrderApiUrl no configurado");
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Order>>($"{_orderApiUrl}/api/orders") 
                ?? new List<Order>();
        }
    }
}