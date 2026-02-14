using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMyFatoorahService
    {
        Task<string> InitiatePayment(decimal amount, string currency = "KWD");
        Task<MyFatoorahPaymentResponse> ExecutePayment(decimal amount, int paymentMethodId, string customerName, string callback, string errorUrl, string customerReference);
        Task<MyFatoorahStatusResponse> GetPaymentStatus(string paymentId);
    }
}
