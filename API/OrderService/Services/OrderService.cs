using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using OrderService.Model;
using OrderService.Data;

namespace OrderService.Services
{
    public class OrderServices
    {
        private readonly OrderDbContext _context;  
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
    
        public OrderServices(OrderDbContext context, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _context = context;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }
    
        public async Task<Order> CreateOrder(Order order)
        {
            // Obtener la URL base de la configuración o usar un valor predeterminado
            var productApiUrl = _configuration["ApiSettings:ProductApiUrl"] ?? "http://product-api";
            
            foreach (var orderProduct in order.Products)
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{productApiUrl}/api/products/{orderProduct.ProductId}");
    
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Producto con ID {orderProduct.ProductId} no encontrado");
                }
    
                var product = await response.Content.ReadFromJsonAsync<Product>();  
                if (product == null || product.Stock < orderProduct.Quantity)
                {
                    throw new Exception($"Producto {orderProduct.ProductId} sin stock suficiente");
                }
                
                product.Stock -= orderProduct.Quantity;
                var updateResponse = await client.PutAsJsonAsync(
                    $"{productApiUrl}/api/products/{orderProduct.ProductId}", 
                    product
                );

                if (!updateResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Error al actualizar el stock del producto {orderProduct.ProductId}");
                }
    
                orderProduct.Price = product.Price;
                orderProduct.ProductName = product.Name; 
                orderProduct.Order = order; 
            }
    
            order.TotalPrice = order.Products.Sum(p => p.Price * p.Quantity);
            order.Status = "Pending";
            order.OrderDate = DateTime.UtcNow;
    
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
    
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Products)
                .ToListAsync();
        }
    
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
