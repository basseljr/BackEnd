using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

namespace Infrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _context;
        private readonly TenantContext _tenantContext;

        public AnalyticsService(AppDbContext context, TenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        // ------------------------------------------------------------
        // SALES SUMMARY ( DAILY / WEEKLY / MONTHLY / CUSTOM )
        // ------------------------------------------------------------

        public async Task<SalesSummaryDto> GetSalesSummaryAsync1(
            string period, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders
                .Where(o => o.TenantId == _tenantContext.TenantId);

            var now = DateTime.UtcNow.Date;

            // Custom range
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate &&
                                         o.CreatedAt <= endDate);
            }
            else
            {
                // Period filtering
                switch (period)
                {
                    case "weekly":
                        var weekStart = now.AddDays(-(int)now.DayOfWeek);
                        query = query.Where(o => o.CreatedAt >= weekStart);
                        break;

                    case "monthly":
                        query = query.Where(o =>
                            o.CreatedAt.Month == now.Month &&
                            o.CreatedAt.Year == now.Year);
                        break;

                    default: // daily
                        query = query.Where(o =>
                            o.CreatedAt.Year == now.Year &&
                            o.CreatedAt.Month == now.Month &&
                            o.CreatedAt.Day == now.Day);
                        break;
                }

            }

            // ---- FIXED GROUPING ----
            var raw = await query
                .GroupBy(o => new
                {
                    o.CreatedAt.Year,
                    o.CreatedAt.Month,
                    o.CreatedAt.Day
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    Total = g.Sum(x => x.Total)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ThenBy(g => g.Day)
                .ToListAsync();

            // Format labels AFTER SQL → SAFE
            var grouped = raw
                .Select(g => new
                {
                    Label = $"{g.Year}-{g.Month:D2}-{g.Day:D2}",
                    g.Total
                })
                .ToList();

            return new SalesSummaryDto
            {
                Labels = grouped.Select(x => x.Label).ToList(),
                Values = grouped.Select(x => x.Total).ToList(),
                Period = period
            };
        }

        public async Task<SalesSummaryDto> GetSalesSummaryAsync(
            string period, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders
                .Where(o => o.TenantId == _tenantContext.TenantId);

            var now = DateTime.UtcNow.Date;

            // Period filters
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate &&
                                         o.CreatedAt <= endDate);
            }
            else
            {
                switch (period)
                {
                    case "weekly":
                        var weekStart = now.AddDays(-(int)now.DayOfWeek);
                        query = query.Where(o => o.CreatedAt >= weekStart);
                        break;

                    case "monthly":
                        query = query.Where(o =>
                            o.CreatedAt.Month == now.Month &&
                            o.CreatedAt.Year == now.Year);
                        break;

                    default: // daily
                        query = query.Where(o => o.CreatedAt.Date == now);
                        break;
                }
            }

            // Return **every order** as a point
            var results = await query
                .OrderBy(o => o.CreatedAt)
                .Select(o => new
                {
                    Label = o.CreatedAt.ToString("HH:mm"),  // show TIME, so points are separate
                    Value = o.Total
                })
                .ToListAsync();

            return new SalesSummaryDto
            {
                Labels = results.Select(r => r.Label).ToList(),
                Values = results.Select(r => r.Value).ToList(),
                Period = period
            };
        }


        // ------------------------------------------------------------
        // TOP ITEMS
        // ------------------------------------------------------------

        public async Task<IEnumerable<TopItemDto>> GetTopItemsAsync(
            int limit, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.OrderItems
                .Include(x => x.Order)
                .Where(x => x.Order.TenantId == _tenantContext.TenantId);

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(x => x.Order.CreatedAt >= startDate &&
                                          x.Order.CreatedAt <= endDate);
            }

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

        // ------------------------------------------------------------
        // ORDER STATUS BREAKDOWN
        // ------------------------------------------------------------

        public async Task<IEnumerable<OrderStatusBreakdownDto>>
            GetOrderStatusBreakdownAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders
                .Where(o => o.TenantId == _tenantContext.TenantId);

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate &&
                                         o.CreatedAt <= endDate);
            }

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

        // ------------------------------------------------------------
        // CUSTOMER ANALYTICS
        // ------------------------------------------------------------

        public async Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(int days)
        {
            var cutoff = DateTime.UtcNow.AddDays(-days);

            var newCustomers = await _context.Users
                .CountAsync(u => u.TenantId == _tenantContext.TenantId &&
                                 u.CreatedAt >= cutoff);

            var total = await _context.Users
                .CountAsync(u => u.TenantId == _tenantContext.TenantId);

            return new CustomerAnalyticsDto
            {
                NewCustomers = newCustomers,
                ReturningCustomers = total - newCustomers,
                TotalCustomers = total
            };
        }
    }
}
