using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrderService.Model
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
    
        [Required]
        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
        [Required]
        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }
    
        [Required]
        [JsonPropertyName("status")]
        public string Status { get; set; } = "Pending";
    
        [Required]
        [JsonPropertyName("products")]
        public List<OrderProduct> Products { get; set; } = new List<OrderProduct>();
    }
    
    public class OrderProduct
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        [JsonIgnore]
        public virtual Order? Order { get; set; }
        
        [Required]
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }
    
        [Required]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [Required]
        [JsonPropertyName("productName")]
        public string ProductName { get; set; }
    
        [Required]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}