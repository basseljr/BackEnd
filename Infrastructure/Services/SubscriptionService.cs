using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using SaaSApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Services
{
 
        public class SubscriptionService : ISubscriptionService
        {
            private readonly AppDbContext _db;
            private readonly IMyFatoorahService _mf;
            private readonly IConfiguration _config;

            public SubscriptionService(AppDbContext db, IMyFatoorahService mf, IConfiguration config)
            {
                _db = db;
                _mf = mf;
                _config = config;
            }

            public async Task<int> CreatePendingSubscriptionAsync(int planId, string email)
            {
                var plan = await _db.SubscriptionPlans.FindAsync(planId);
                if (plan == null)
                    throw new Exception("Plan not found");

                var subscription = new Subscription
                {
                    TenantId = null,
                    PlanName = plan.Name,
                    Price = plan.PriceMonthly,
                    BillingCycle = "Monthly",
                    Status = "Pending",
                    StartDate = DateTime.UtcNow,
                    TransactionId = null,
                    InvoiceId = null
                };

                _db.Subscriptions.Add(subscription);
                await _db.SaveChangesAsync();

                return subscription.Id;
            }

            public async Task<string> StartSubscriptionAsync(StartSubscriptionRequest request, string customerEmail)
            {
                // 1. Create pending subscription
                int subscriptionId = await CreatePendingSubscriptionAsync(request.PlanId, customerEmail);

                // 2. Initiate payment to get available methods
                var paymentMethodsJson = await _mf.InitiatePayment(request.InvoiceValue);

                int paymentMethodId = 2; // VISA for now

            var callback = _config["MyFatoorah:SubscriptionCallbackUrl"];
            var error = _config["MyFatoorah:ErrorUrl"];


            // 3. Execute payment INCLUDING CustomerReference
            var response = await _mf.ExecutePayment(
                    request.InvoiceValue,
                    paymentMethodId,
                    customerEmail,
                    callback,
                    error,
                    subscriptionId.ToString() // <-- VERY IMPORTANT
                );

                // 4. Save invoiceId returned from MF
                var subscription = await _db.Subscriptions.FindAsync(subscriptionId);
                subscription.InvoiceId = response.Data.InvoiceId.ToString();
                subscription.PaymentMethod = paymentMethodId.ToString();

                await _db.SaveChangesAsync();

                return response.Data.PaymentURL;
            }

            public async Task<bool> HandleCallbackAsync1(string paymentId)
            {
                var status = await _mf.GetPaymentStatus(paymentId);

                if (status.Data.InvoiceStatus != "Paid")
                    return false;

                int subscriptionId = int.Parse(status.Data.CustomerReference);
                string customerEmail = status.Data.CustomerEmail;

                var subscription = await _db.Subscriptions.FindAsync(subscriptionId);
                if (subscription == null) return false;

                subscription.Status = "Active";

                // Create tenant
                var user = await _db.Users.FirstAsync(u => u.Email == customerEmail);

                var tenant = new Tenant
                {
                    Name = user.Email.Split('@')[0],
                    Subdomain = GenerateSubdomain(user.Email),
                    TemplateId = 1, // TODO: real value
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Tenants.Add(tenant);
                await _db.SaveChangesAsync();

                user.TenantId = tenant.Id;
                subscription.TenantId = tenant.Id;

                await _db.SaveChangesAsync();

                return true;
            }


        public async Task<SubscriptionCallbackResult> HandleCallbackAsync(string paymentId)
        {
            var status = await _mf.GetPaymentStatus(paymentId);

            if (status == null || status.Data == null)
                return SubscriptionCallbackResult.Failed("Invalid payment status response");

            if (!string.Equals(status.Data.InvoiceStatus, "Paid", StringComparison.OrdinalIgnoreCase))
                return SubscriptionCallbackResult.Failed("Payment not completed");

            // Safe parse
            if (!int.TryParse(status.Data.CustomerReference, out int subscriptionId))
                return SubscriptionCallbackResult.Failed("Invalid subscription reference");

            var subscription = await _db.Subscriptions.FindAsync(subscriptionId);
            if (subscription == null)
                return SubscriptionCallbackResult.Failed("Subscription not found");

            // 🔒 Idempotency check
            if (subscription.Status == "Active" && subscription.TenantId.HasValue)
            {
                var existingTenant = await _db.Tenants.FindAsync(subscription.TenantId.Value);
                return SubscriptionCallbackResult.AlreadyProcessed(
                    subscription.Id,
                    existingTenant!.Id,
                    existingTenant.Subdomain
                );
            }

            subscription.Status = "Active";

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == status.Data.CustomerEmail);
            if (user == null)
                return SubscriptionCallbackResult.Failed("User not found");

            // Create tenant
            var tenant = new Tenant
            {
                Name = user.Email.Split('@')[0],
                Subdomain = GenerateSubdomain(user.Email),
                TemplateId = 1, 
                IsPublished = true,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync();

            // Link everything
            user.TenantId = tenant.Id;
            subscription.TenantId = tenant.Id;

            await _db.SaveChangesAsync();

            return SubscriptionCallbackResult.Success(
                subscription.Id,
                tenant.Id,
                tenant.Subdomain
            );
        }

        private string GenerateSubdomain(string email)
            {
                return email.Split('@')[0].ToLower();
            }


        public async Task<List<SubscriptionPlanDto>> GetPlansAsync()
        {
            return await _db.SubscriptionPlans
                .Select(p => new SubscriptionPlanDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    PriceMonthly = p.PriceMonthly,
                    PriceYearly = p.PriceYearly
                })
                .ToListAsync();
        }



        public async Task ProcessWebhookAsync(PaymentWebhookEvent webhookEvent)
        {
            if (!int.TryParse(webhookEvent.ReferenceId.Replace("SUB-", ""), out var subscriptionId))
                return;

            var subscription = await _db.Subscriptions
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription == null)
                return;

            // 🔐 Transition Guard
            if (!IsValidSubscriptionTransition(subscription.Status, webhookEvent.EventType))
            {
                //_logger.LogWarning(
                //    "Invalid subscription transition blocked | SubId={SubId} Current={Current} Incoming={Incoming}",
                //    subscription.Id,
                //    subscription.Status,
                //    webhookEvent.EventType
                //);
                return;
            }

            switch (webhookEvent.EventType)
            {
                case "PaymentPaid":

                    // If subscription was Grace or Failed or Expired → restore it
                    if (subscription.Status != "Active" &&
                        subscription.Status != "Grace")
                    {
                        subscription.Status = "Active";

                        // Restore from Grace
                        subscription.GraceEndDate = null;

                        // Use gateway timestamp (never DateTime.Now)
                        subscription.ActivatedAt ??= webhookEvent.OccurredAt;

                        if (subscription.StartDate == null)
                            subscription.StartDate = webhookEvent.OccurredAt;

                        if (subscription.EndDate == null)
                        {
                            // TODO: Replace 1 with Plan.DurationMonths
                            subscription.EndDate =
                                webhookEvent.OccurredAt.AddMonths(1);
                        }
                    }

                    break;

                case "PaymentFailed":
                case "PaymentExpired":

                    if (subscription.Status != "Active")
                    {
                        subscription.Status = "Failed";
                    }

                    break;

                case "PaymentRefunded":

                    if (subscription.Status == "Active")
                    {
                        subscription.Status = "Cancelled";
                        subscription.EndDate = webhookEvent.OccurredAt;
                    }
                    break;


            
//⚠️ Important Improvement You Must Do Next

//Right now you hardcoded:

//AddMonths(1)


//This is not production ready.

//You must replace it with:

//            var plan = await _db.SubscriptionPlans
//                .FirstOrDefaultAsync(p => p.Id == subscription.PlanId);
//            subscription.EndDate =
//                webhookEvent.OccurredAt.AddMonths(plan.DurationInMonths);


//            Otherwise all plans become 1 month.
            }



            await _db.SaveChangesAsync();
        }



        private bool IsValidSubscriptionTransition(string currentStatus, string incomingEvent)
        {
            // Active subscriptions should not downgrade
            if (currentStatus == "Active" &&
                (incomingEvent == "PaymentFailed" || incomingEvent == "PaymentExpired"))
                return false;

            // Prevent repeated same state updates
            if (currentStatus == "Failed" && incomingEvent == "PaymentFailed")
                return false;

            if (currentStatus == "Expired" && incomingEvent == "PaymentExpired")
                return false;

            return true;
        }


    }

}

