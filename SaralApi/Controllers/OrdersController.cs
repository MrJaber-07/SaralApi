using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using SaralApi.DTOs.Orders;
using SaralApi.Interface;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    // This is the Constructor. .NET automatically injects the service here.
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        if (!Request.Headers.TryGetValue("X-Idempotency-Key", out var key))
            return BadRequest("Idempotency Key is required.");

        // Use _orderService (the field), not orderService (local variable)
        var result = await _orderService.PlaceOrderAsync(request, key.ToString());

        return Ok(new { Message = result });
    }
}