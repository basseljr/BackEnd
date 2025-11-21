using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /Analytics/sales?period=daily
        [HttpGet("sales")]
        public IActionResult GetSalesSummary([FromQuery] string period = "daily")
        {
            // Dummy example – replace with real query logic
            var result = new
            {
                labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri" },
                values = new[] { 120, 200, 180, 250, 300 }
            };
            return Ok(result);
        }

        // GET /Analytics/top-items?limit=5
        [HttpGet("top-items")]
        public IActionResult GetTopItems([FromQuery] int limit = 5)
        {
            var items = _context.Items
                .OrderByDescending(i => i.Price)
                .Take(limit)
                .Select(i => new
                {
                    itemName = i.Name,
                    quantitySold = 50,
                    revenue = i.Price * 50
                })
                .ToList();

            return Ok(items);
        }

        // GET /Analytics/status-breakdown
        [HttpGet("status-breakdown")]
        public IActionResult GetOrderStatusBreakdown()
        {
            var breakdown = _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    status = g.Key,
                    count = g.Count()
                })
                .ToList();

            return Ok(breakdown);
        }

        // GET /Analytics/customers
        [HttpGet("customers")]
        public IActionResult GetCustomerAnalytics()
        {
            var newCustomers = _context.Users.Count(c => c.CreatedAt > DateTime.UtcNow.AddDays(-30));
            var returningCustomers = _context.Users.Count() - newCustomers;

            return Ok(new { newCustomers, returningCustomers, totalCustomers = newCustomers + returningCustomers });
        }
    }
}
