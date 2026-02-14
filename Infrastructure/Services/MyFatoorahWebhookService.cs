using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaaSApp.Infrastructure.Data;

namespace Infrastructure.Services
{
    public class MyFatoorahWebhookService : IMyFatoorahWebhookService
    {
        private readonly AppDbContext _db;
        private readonly IPaymentSignatureValidator _signatureValidator;
        private readonly IOrderService _orderService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPaymentWebhookAuditService _audit;

        public MyFatoorahWebhookService(
            AppDbContext db,
            IPaymentSignatureValidator signatureValidator,
            IOrderService orderService,
            ISubscriptionService subscriptionService,
            IPaymentWebhookAuditService audit)
        {
            _db = db;
            _signatureValidator = signatureValidator;
            _orderService = orderService;
            _subscriptionService = subscriptionService;
            _audit = audit;
        }
        public async Task ProcessAsync(string rawJson, string? signature)
        {
            var dto = JsonConvert.DeserializeObject<MyFatoorahWebhookDto>(rawJson);
            if (dto?.Data == null)
                return;

            string referenceId = dto.Data.CustomerReference;
            string eventType = dto.Event;

            if (string.IsNullOrEmpty(referenceId))
                return;

            // STEP 1 — Resolve Tenant
            int tenantId = await ResolveTenantId(referenceId);
            if (tenantId == 0)
                return;

            // STEP 2 — Load Secret
            var settings = await _db.TenantPaymentSettings
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            string? secretKey = settings?.SecretKey;

            // STEP 3 — Validate Signature
            if (!string.IsNullOrEmpty(signature))
            {
                var isValid = _signatureValidator.Validate(rawJson, signature, secretKey ?? "");
                if (!isValid)
                    return;
            }

            PaymentWebhookEvent webhookEntity;

            try
            {
                // STEP 4 — Save First (DB enforces uniqueness)
                webhookEntity = await _audit.CreateAsync(new PaymentWebhookEvent
                {
                    Gateway = "MyFatoorah",
                    ReferenceId = referenceId,
                    EventType = eventType,
                    InvoiceId = dto.Data.InvoiceId,
                    Amount = dto.Data.InvoiceValue,
                    OccurredAt = dto.DateTime,
                    PayloadJson = rawJson,
                    ReceivedAt = DateTime.UtcNow,
                    IsProcessed = false
                });
            }
            catch (DbUpdateException)
            {
                // Duplicate webhook → safely ignore
                return;
            }

            try
            {
                var normalized = Normalize(dto);

                if (IsOrderReference(referenceId))
                    await _orderService.ProcessWebhookAsync(normalized);
                else
                    await _subscriptionService.ProcessWebhookAsync(normalized);

                await _audit.MarkProcessedAsync(webhookEntity.Id);
            }
            catch (Exception ex)
            {
                await _audit.MarkFailedAsync(webhookEntity.Id, ex.Message);
            }
        }



        private async Task<int> ResolveTenantId(string referenceId)
        {
            if (IsOrderReference(referenceId))
            {
                var orderId = int.Parse(referenceId.Replace("ORDER-", ""));
                var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                return order?.TenantId ?? 0;
            }
            else
            {
                var subscriptionId = int.Parse(referenceId.Replace("SUB-", ""));
                var subscription = await _db.Subscriptions
                    .FirstOrDefaultAsync(s => s.Id == subscriptionId);
                return subscription?.TenantId ?? 0;
            }
        }

        private bool IsOrderReference(string referenceId)
        {
            return referenceId.StartsWith("ORDER-");
        }

        private PaymentWebhookEvent Normalize(MyFatoorahWebhookDto dto)
        {
            return new PaymentWebhookEvent
            {
                Gateway = "MyFatoorah",
                ReferenceId = dto.Data.CustomerReference,
                InvoiceId = dto.Data.InvoiceId,
                Amount = dto.Data.InvoiceValue,
                OccurredAt = dto.DateTime,
                EventType = dto.Data.InvoiceStatus switch
                {
                    "Paid" => "PaymentPaid",
                    "Expired" => "PaymentExpired",
                    _ => "PaymentFailed"
                }
            };
        }
    }
}
