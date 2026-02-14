using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StartSubscriptionRequest
    {
        public int PlanId { get; set; }
        public decimal InvoiceValue { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = "Monthly";
    }
}
