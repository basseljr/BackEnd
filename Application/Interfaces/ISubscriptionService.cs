using Application.Common;
using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionPlanDto>> GetPlansAsync();
        Task<int> CreatePendingSubscriptionAsync(int planId, string email);
        Task<string> StartSubscriptionAsync(StartSubscriptionRequest request, string customerEmail);
        Task<SubscriptionCallbackResult> HandleCallbackAsync(string paymentId);
        Task ProcessWebhookAsync(PaymentWebhookEvent webhookEvent);


    }

}
