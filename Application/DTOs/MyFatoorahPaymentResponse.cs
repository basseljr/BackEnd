using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MyFatoorahPaymentResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public PaymentResponseData Data { get; set; }
    }
}
