using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;
using System.Net.Http;
using System.Security.Claims;

namespace Infrastructure.Middleware
{
    public class SubscriptionGuardMiddleware
    {
        private readonly RequestDelegate _next;

        public SubscriptionGuardMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            // Skip public endpoints
            if (context.Request.Path.StartsWithSegments("/api/webhooks") ||
                context.Request.Path.StartsWithSegments("/api/auth"))
            {
                await _next(context);
                return;
            }

            var tenantIdClaim = context.User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim))
            {
                await _next(context);
                return;
            }

            int tenantId = int.Parse(tenantIdClaim);

            var subscription = await db.Subscriptions
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync(s => s.TenantId == tenantId);

            if (subscription == null)
            {
                context.Response.StatusCode = 402;
                await context.Response.WriteAsync("Subscription required.");
                return;
            }

            if (subscription.Status != "Active")
            {
                context.Response.StatusCode = 402;
                await context.Response.WriteAsync("Subscription expired.");
                return;
            }

            await _next(context);
        }
    }
}
