using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{

    public class PaymentTransaction
    {
        public string TransactionId { get; set; }
        public string PaymentGateway { get; set; }
        public string TransactionStatus { get; set; }
        public string PaidCurrency { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
