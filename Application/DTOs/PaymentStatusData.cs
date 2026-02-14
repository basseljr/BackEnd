using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PaymentStatusData
    {
        public int? InvoiceId { get; set; }
        public string InvoiceStatus { get; set; }   // Paid, Failed, Pending
        public string InvoiceValue { get; set; }
        public List<PaymentTransaction>? InvoiceTransactions { get; set; }
        public string CustomerReference { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
