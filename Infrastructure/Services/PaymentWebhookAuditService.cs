using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

namespace Infrastructure.Services
{
    public class PaymentWebhookAuditService : IPaymentWebhookAuditService
    {
        private readonly AppDbContext _db;

        public PaymentWebhookAuditService(AppDbContext db)
        {
            _db = db;
        }

        //public async Task<PaymentWebhookEvent> CreateAsync1(
        //    string eventType,
        //    string? invoiceId,
        //    string? paymentId,
        //    string payloadJson)
        //{
        //    var entity = new PaymentWebhookEvent
        //    {
        //        EventType = eventType,
        //        InvoiceId = invoiceId,
        //        PaymentId = paymentId,
        //        PayloadJson = payloadJson,
        //        ReceivedAt = DateTime.UtcNow,
        //        IsProcessed = false
        //    };

        //    _db.PaymentWebhookEvents.Add(entity);
        //    await _db.SaveChangesAsync();

        //    return entity;
        //}

        public async Task<PaymentWebhookEvent> CreateAsync(PaymentWebhookEvent entity)
        {
            _db.PaymentWebhookEvents.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ExistsAsync(string gateway, string referenceId, string eventType)
        {
            return await _db.PaymentWebhookEvents.AnyAsync(x =>
                x.Gateway == gateway &&
                x.ReferenceId == referenceId &&
                x.EventType == eventType
            );
        }


        public async Task MarkProcessedAsync(int webhookEventId)
        {
            var entity = await _db.PaymentWebhookEvents.FindAsync(webhookEventId);
            if (entity == null) return;

            entity.IsProcessed = true;
            entity.ProcessedAt = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async Task MarkFailedAsync(int webhookEventId, string error)
        {
            var entity = await _db.PaymentWebhookEvents.FindAsync(webhookEventId);
            if (entity == null) return;

            entity.ErrorMessage = error;
            entity.IsProcessed = false;
            entity.ProcessedAt = null;

            await _db.SaveChangesAsync();
        }

    }
}
