using Application.DTOs;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

namespace Infrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _context;
        public AnalyticsService(AppDbContext context) => _context = context;

        public async Task<SalesSummaryDto> GetSalesSummaryAsync(
            int tenantId, string period, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders.Where(o => o.TenantId == tenantId);

            if (startDate.HasValue && endDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate &&
                                         o.CreatedAt <= endDate);

            var now = DateTime.UtcNow.Date;

            if (!startDate.HasValue || !endDate.HasValue)
            {
                if (period == "weekly")
                {
                    var weekStart = now.AddDays(-(int)now.DayOfWeek);
                    query = query.Where(o => o.CreatedAt >= weekStart);
                }
                else if (period == "monthly")
                {
                    query = query.Where(o =>
                        o.CreatedAt.Month == now.Month &&
                        o.CreatedAt.Year == now.Year
                    );
                }
                else
                {
                    query = query.Where(o => o.CreatedAt.Date == now);
                }
            }

            var grouped = await query
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Total = g.Sum(x => x.Total)
                })
                .OrderBy(x => x.Label)
                .ToListAsync();

            return new SalesSummaryDto
            {
                Labels = grouped.Select(x => x.Label).ToList(),
                Values = grouped.Select(x => x.Total).ToList(),
                Period = period
            };
        }

        public async Task<IEnumerable<TopItemDto>> GetTopItemsAsync(
            int tenantId, int limit, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.OrderItems
                .Include(x => x.Order)
                .Include(x => x.Item)
                .Where(x => x.Order.TenantId == tenantId);

            if (startDate.HasValue && endDate.HasValue)
                query = query.Where(x => x.Order.CreatedAt >= startDate &&
                                         x.Order.CreatedAt <= endDate);

            return await query
                .GroupBy(x => new { x.ItemId, x.ItemName })
                .Select(g => new TopItemDto
                {
                    ItemId = g.Key.ItemId ?? 0,
                    ItemName = g.Key.ItemName,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Price * x.Quantity)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderStatusBreakdownDto>>
            GetOrderStatusBreakdownAsync(int tenantId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders.Where(o => o.TenantId == tenantId);

            if (startDate.HasValue && endDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate &&
                                         o.CreatedAt <= endDate);

            var total = await query.CountAsync();
            if (total == 0) total = 1;

            return await query
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusBreakdownDto
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Percentage = (double)g.Count() / total * 100
                })
                .ToListAsync();
        }

        public async Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(
            int tenantId, int days)
        {
            var cutoff = DateTime.UtcNow.AddDays(-days);

            var newCustomers = await _context.Users
                .CountAsync(u => u.TenantId == tenantId &&
                                 u.CreatedAt >= cutoff);

            var total = await _context.Users
                .CountAsync(u => u.TenantId == tenantId);

            return new CustomerAnalyticsDto
            {
                NewCustomers = newCustomers,
                ReturningCustomers = total - newCustomers,
                TotalCustomers = total
            };
        }
    }
}
