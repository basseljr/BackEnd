using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MyFatoorahWebhookData
    {
        public string InvoiceId { get; set; } = string.Empty;
        public string InvoiceReference { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public decimal InvoiceValue { get; set; }
        public string CustomerReference { get; set; } = string.Empty;
        public string SubscriptionId { get; set; } = string.Empty;
        public string InvoiceStatus { get; set; } = string.Empty;

    }
}
