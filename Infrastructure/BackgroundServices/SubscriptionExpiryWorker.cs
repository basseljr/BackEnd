using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SaaSApp.Infrastructure.Data;

namespace Infrastructure.BackgroundServices
{
    public class SubscriptionExpiryWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscriptionExpiryWorker> _logger;

        public SubscriptionExpiryWorker(
            IServiceProvider serviceProvider,
            ILogger<SubscriptionExpiryWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Expiry Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    int graceDays = config.GetValue<int>("SubscriptionSettings:GracePeriodDays");
                    var now = DateTime.UtcNow;

                    // STEP 1 — Move Active → Grace
                    var toGrace = await db.Subscriptions
                        .Where(s =>
                            s.Status == "Active" &&
                            s.EndDate != null &&
                            s.EndDate <= now)
                        .ToListAsync(stoppingToken);

                    foreach (var subscription in toGrace)
                    {
                        subscription.Status = "Grace";
                        subscription.GraceEndDate = now.AddDays(graceDays);

                        _logger.LogInformation(
                            "Subscription moved to Grace | Id={Id}",
                            subscription.Id);
                    }

                    // STEP 2 — Move Grace → Expired
                    var toExpire = await db.Subscriptions
                        .Where(s =>
                            s.Status == "Grace" &&
                            s.GraceEndDate != null &&
                            s.GraceEndDate <= now)
                        .ToListAsync(stoppingToken);

                    foreach (var subscription in toExpire)
                    {
                        subscription.Status = "Expired";

                        _logger.LogInformation(
                            "Subscription expired permanently | Id={Id}",
                            subscription.Id);
                    }

                    if (toGrace.Any() || toExpire.Any())
                        await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SubscriptionExpiryWorker");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
