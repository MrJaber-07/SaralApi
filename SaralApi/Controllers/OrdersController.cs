using Microsoft.AspNetCore.Mvc;
using SaralApi.DTOs.Orders;
using SaralApi.Models;
using SaralApi.Services;

namespace SaralApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // 1. CREATE: Place a new order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            if (!Request.Headers.TryGetValue("X-Idempotency-Key", out var key))
            {
                return BadRequest("X-Idempotency-Key header is required for retry safety.");
            }

            var result = await _orderService.PlaceOrderAsync(request, key.ToString());

            if (result.Contains("Duplicate") || result.Contains("Failed"))
            {
                return BadRequest(new { Message = result });
            }

            return Ok(new { Message = result });
        }

        // 2. READ (ALL): Get all orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // 3. READ (SINGLE): Get order details by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound($"Order with ID {id} not found.");

            return Ok(order);
        }

        // 4. UPDATE: Modify an existing order (e.g., change status manually)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updatedOrder)
        {
            if (id != updatedOrder.Id) return BadRequest("ID mismatch.");

            var success = await _orderService.UpdateOrderAsync(updatedOrder);
            if (!success) return NotFound();

            return NoContent();
        }

        // 5. DELETE: Remove an order record
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success) return NotFound();

            return Ok(new { Message = $"Order {id} deleted successfully." });
        }
    }
}