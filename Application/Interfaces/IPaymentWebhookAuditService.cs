using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    
        public interface IPaymentWebhookAuditService
        {
        Task<PaymentWebhookEvent> CreateAsync(PaymentWebhookEvent entity);

        Task MarkProcessedAsync(int webhookEventId);

            Task MarkFailedAsync(int webhookEventId, string errorMessage);
        Task<bool> ExistsAsync(string gateway, string referenceId, string eventType);

        }
}

