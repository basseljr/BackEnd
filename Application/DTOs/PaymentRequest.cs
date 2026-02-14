using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "KWD";
        public string CustomerEmail { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string CustomerReference { get; set; } = ""; // subscriptionId or orderId
        public string CallbackUrl { get; set; } = "";
        public string ErrorUrl { get; set; } = "";
    }
}
