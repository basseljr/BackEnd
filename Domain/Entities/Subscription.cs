using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string PlanName { get; set; } = string.Empty; // e.g., Basic, Pro, Enterprise
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } = "Monthly"; // Monthly / Yearly
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled
        public string? PaymentGateway { get; set; } // Tap, MyFatoorah, etc.
        public string? TransactionId { get; set; }

        public string? PaymentToken { get; set; } // Optional future use
        public DateTime? NextBillingDate { get; set; }
        public string? InvoiceId { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? ActivatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime? GraceEndDate { get; set; }

        public Tenant? Tenant { get; set; }
    }

}
