using ProductCrudBlazor.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace ProductCrudBlazor.Services
{
    public class CartService
    {
        private readonly HttpClient _httpClient;
        public event Action OnCartChanged;
        private readonly string _orderApiUrl;

        public CartService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _orderApiUrl = configuration["ApiSettings:OrderApiUrl"] 
                ?? throw new InvalidOperationException("OrderApiUrl no configurado");
        }

        public async Task<Cart> GetCartAsync()
        {
            const string userId = "default-user";
            return await _httpClient.GetFromJsonAsync<Cart>($"{_orderApiUrl}/api/cart/{userId}") 
                ?? new Cart { UserId = userId };
        }

        public async Task AddToCart(Product product, int quantity)
        {
            const string userId = "default-user";
            var content = JsonContent.Create(new { productId = product.Id, quantity });
            await _httpClient.PostAsync($"{_orderApiUrl}/api/cart/{userId}/items", content);
            
            OnCartChanged?.Invoke();
        }

        public async Task UpdateCartItemQuantity(int productId, int quantity)
        {
            const string userId = "default-user";
            var content = JsonContent.Create(new { quantity });
            await _httpClient.PutAsync(
                $"{_orderApiUrl}/api/cart/{userId}/items/{productId}",
                content);
            
            OnCartChanged?.Invoke();
        }

        public async Task RemoveFromCart(int productId)
        {
            await UpdateCartItemQuantity(productId, 0);
            OnCartChanged?.Invoke();
        }

        public async Task ClearCart()
        {
            const string userId = "default-user";
            await _httpClient.DeleteAsync($"{_orderApiUrl}/api/cart/{userId}");
            OnCartChanged?.Invoke();
        }

        public async Task<Order> CreateOrder()
        {
            var cart = await GetCartAsync();
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                Products = cart.Items.Select(item => new OrderProduct
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ProductName = item.ProductName
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync($"{_orderApiUrl}/api/orders", order);
            
            if (response.IsSuccessStatusCode)
            {
                var createdOrder = await response.Content.ReadFromJsonAsync<Order>();
                await ClearCart();
                return createdOrder;
            }

            throw new Exception("Failed to create order");
        }
    }
}