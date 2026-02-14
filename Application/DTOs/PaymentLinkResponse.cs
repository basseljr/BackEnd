using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{

    public class PaymentLinkResponse
    {
        public string InvoiceId { get; set; } = "";
        public string PaymentUrl { get; set; } = "";
    }
}
