using Application.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin,Owner")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly TenantContext _tenantContext;

        public AnalyticsController(IAnalyticsService analyticsService, TenantContext tenantContext)
        {
            _analyticsService = analyticsService;
            _tenantContext = tenantContext;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSales(
            [FromQuery] string period = "daily",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _analyticsService.GetSalesSummaryAsync(
                period, startDate, endDate
            );
            return Ok(result);
        }

        [HttpGet("top-items")]
        public async Task<IActionResult> GetTopItems(
            [FromQuery] int limit = 5,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _analyticsService.GetTopItemsAsync(
                limit, startDate, endDate
            );
            return Ok(result);
        }

        [HttpGet("status-breakdown")]
        public async Task<IActionResult> GetStatusBreakdown(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _analyticsService.GetOrderStatusBreakdownAsync(
                startDate, endDate
            );
            return Ok(result);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomerAnalytics(
            [FromQuery] int days = 30)
        {
            var result = await _analyticsService.GetCustomerAnalyticsAsync(
                days
            );
            return Ok(result);
        }
    }
}
