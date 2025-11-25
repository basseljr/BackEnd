using Application.Common;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

namespace SaaSApp.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TenantContext tenantContext, AppDbContext dbContext)
        {
            // Skip tenant validation for Auth endpoints
            if (context.Request.Path.StartsWithSegments("/Auth"))
            {
                await _next(context);
                return;
            }

            // Get X-Tenant-Id header
            if (!context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { message = "X-Tenant-Id header is required" });
                return;
            }

            // Parse tenant ID
            if (!int.TryParse(tenantIdHeader, out int tenantId) || tenantId <= 0)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid X-Tenant-Id header value" });
                return;
            }

            // Validate tenant exists and is active
            var tenantExists = await dbContext.Tenants
                .AnyAsync(t => t.Id == tenantId && t.IsActive);

            if (!tenantExists)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new { message = "Tenant not found or inactive" });
                return;
            }

            // Set tenant context
            tenantContext.TenantId = tenantId;

            await _next(context);
        }
    }
}
