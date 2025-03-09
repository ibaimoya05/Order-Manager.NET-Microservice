using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Model;
using System.Net.Http.Json;

namespace OrderService.Services
{
    public class CartService
    {
        private readonly OrderDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public CartService(OrderDbContext context, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _context = context;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<Cart> GetCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart> AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartAsync(userId);
            var client = _clientFactory.CreateClient();
            
            // Obtener la URL base de la configuración o usar un valor predeterminado
            var productApiUrl = _configuration["ApiSettings:ProductApiUrl"] ?? "http://product-api";
            
            var response = await client.GetAsync($"{productApiUrl}/api/products/{productId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Producto con ID {productId} no encontrado");
            }

            var product = await response.Content.ReadFromJsonAsync<Product>();
            if (product == null)
            {
                throw new Exception($"Producto {productId} no encontrado");
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.Price = product.Price;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    ProductName = product.Name,
                    Price = product.Price,
                    Cart = cart
                });
            }

            cart.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateCartItemQuantityAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (item == null)
                throw new Exception("Producto no encontrado en el carrito");

            if (quantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = quantity;

            cart.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            cart.Items.Clear();
            cart.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}