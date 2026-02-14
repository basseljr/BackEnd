using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SaaSApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundServices
{
    public class WebhookRetryWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WebhookRetryWorker> _logger;

        public WebhookRetryWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<WebhookRetryWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var webhookService = scope.ServiceProvider
                        .GetRequiredService<IMyFatoorahWebhookService>();

                    var failedWebhooks = await db.PaymentWebhookEvents
                        .Where(x => x.IsFailed && x.RetryCount < 5)
                        .OrderBy(x => x.ReceivedAt)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var webhook in failedWebhooks)
                    {
                        try
                        {
                            await webhookService.ProcessAsync(webhook.PayloadJson, null);

                            webhook.IsFailed = false;
                            webhook.IsProcessed = true;
                            webhook.ProcessedAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            webhook.RetryCount++;
                            webhook.LastRetryAt = DateTime.UtcNow;
                            webhook.ErrorMessage = ex.Message;
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Webhook retry worker crashed");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
