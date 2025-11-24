using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return BadRequest("Invalid order data.");

            var orderId = await _orderService.CreateOrderAsync(request);
            return Ok(new { orderId, message = "Order placed successfully" });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int tenantId)
        {
            var orders = await _orderService.GetAllAsync(tenantId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id, [FromQuery] int tenantId)
        {
            var order = await _orderService.GetByIdAsync(id, tenantId);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory([FromQuery] string mobile, [FromQuery] int tenantId)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return BadRequest("Mobile number is required.");

            var orders = await _orderService.GetByCustomerMobileAsync(mobile, tenantId);
            return Ok(orders);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request, [FromQuery] int tenantId)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, request.Status, tenantId);
            if (!success)
                return NotFound(new { message = "Order not found" });

            return Ok(new { message = "Order status updated successfully" });
        }




    }
}
