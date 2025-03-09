using Microsoft.AspNetCore.Mvc;
using OrderService.Model;
using OrderService.Services;


namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderServices _orderServices;

        public OrdersController(OrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderServices.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderServices.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            try
            {
                var newOrder = await _orderServices.CreateOrder(order);
                return CreatedAtAction("GetOrder", new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> GetCart(string userId)
        {
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        // POST: api/cart/{userId}/items
        [HttpPost("{userId}/items")]
        public async Task<ActionResult<Cart>> AddToCart(string userId, [FromBody] CartItemRequest request)
        {
            var cart = await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
            return Ok(cart);
        }

        // PUT: api/cart/{userId}/items/{productId}
        [HttpPut("{userId}/items/{productId}")]
        public async Task<ActionResult<Cart>> UpdateQuantity(string userId, int productId, [FromBody] UpdateQuantityRequest request)
        {
            var cart = await _cartService.UpdateCartItemQuantityAsync(userId, productId, request.Quantity);
            return Ok(cart);
        }

        // DELETE: api/cart/{userId}
        [HttpDelete("{userId}")]
        public async Task<ActionResult> ClearCart(string userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok();
        }
    }

    public class CartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateQuantityRequest
    {
        public int Quantity { get; set; }
    }
}