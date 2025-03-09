using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.Model 
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("items")]
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class CartItem 
    {
        public int Id { get; set; }
        
        public int CartId { get; set; }
        
        [JsonIgnore]
        public Cart? Cart { get; set; }
        
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