using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MyFatoorahStatusResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public PaymentStatusData Data { get; set; }
    }

}
