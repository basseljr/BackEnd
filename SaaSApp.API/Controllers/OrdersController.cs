using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly TenantContext _tenantContext;
        public OrdersController(IOrderService orderService, TenantContext tenantContext)
        {
            _orderService = orderService;
            _tenantContext = tenantContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return BadRequest("Invalid order data.");

            var orderId = await _orderService.CreateOrderAsync(request);
            return Ok(new { orderId, message = "Order placed successfully" });
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,Owner,Customer")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory([FromQuery] string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return BadRequest("Mobile number is required.");

            var orders = await _orderService.GetByCustomerMobileAsync(mobile);
            return Ok(orders);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Owner,Customer")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, request.Status);
            if (!success)
                return NotFound(new { message = "Order not found" });

            return Ok(new { message = "Order status updated successfully" });
        }




    }
}
