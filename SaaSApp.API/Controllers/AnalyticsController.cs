using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSales(
            [FromQuery] int tenantId,
            [FromQuery] string period = "daily",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _analyticsService.GetSalesSummaryAsync(
                tenantId, period, startDate, endDate
            );
            return Ok(result);
        }

        [HttpGet("top-items")]
        public async Task<IActionResult> GetTopItems(
            [FromQuery] int tenantId,
            [FromQuery] int limit = 5,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _analyticsService.GetTopItemsAsync(
                tenantId, limit, startDate, endDate
            );
            return Ok(result);
        }

        [HttpGet("status-breakdown")]
        public async Task<IActionResult> GetStatusBreakdown(
            [FromQuery] int tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _analyticsService.GetOrderStatusBreakdownAsync(
                tenantId, startDate, endDate
            );
            return Ok(result);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomerAnalytics(
            [FromQuery] int tenantId,
            [FromQuery] int days = 30)
        {
            var result = await _analyticsService.GetCustomerAnalyticsAsync(
                tenantId, days
            );
            return Ok(result);
        }
    }
}
