using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PaymentResponseData
    {
        public int InvoiceId { get; set; }
        public bool IsDirectPayment { get; set; }
        public string PaymentURL { get; set; }
        public string CustomerReference { get; set; }
        public string UserDefinedField { get; set; }
    }
}
