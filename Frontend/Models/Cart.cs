using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ProductCrudBlazor.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime LastUpdated { get; set; }
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