using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
 
        public interface IPaymentGateway
        {
            Task<PaymentLinkResponse> CreatePaymentLink(PaymentRequest request);
            Task<PaymentStatusResponse> GetPaymentStatus(string paymentId);
        }
    
}
