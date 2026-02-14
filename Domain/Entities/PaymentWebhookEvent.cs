
namespace Domain.Entities
{

    public class PaymentWebhookEvent
    {
        public int Id { get; set; }

        // Which gateway sent this webhook (e.g., MyFatoorah)
        public string Gateway { get; set; } = string.Empty;

        // OrderId or SubscriptionId (stored as string to stay generic)
        public string ReferenceId { get; set; } = string.Empty;

        // Gateway invoice / transaction identifiers
        public string? InvoiceId { get; set; }
        public string? PaymentId { get; set; }

        // Event type: PaymentPaid, PaymentFailed, RefundCompleted, etc.
        public string EventType { get; set; } = string.Empty;

        // Amount paid / refunded
        public decimal Amount { get; set; }

        // Raw webhook payload (JSON) for audit & debugging
        // Primary storage property used by services
        public string PayloadJson { get; set; } = string.Empty;

        // Backwards-compatible alias for payload, if used elsewhere
        public string Payload
        {
            get => PayloadJson;
            set => PayloadJson = value;
        }

        // When the event occurred according to the gateway
        public DateTime OccurredAt { get; set; }

        // When our system received the webhook
        public DateTime ReceivedAt { get; set; } = DateTime.Now;

        // When we finished processing (success or failure)
        public DateTime? ProcessedAt { get; set; }

        // Processing flags
        public bool IsProcessed { get; set; }

        // Error details if processing failed
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; }
        public DateTime? LastRetryAt { get; set; }
        public bool IsFailed { get; set; }
    }
}