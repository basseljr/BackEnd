using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class SubscriptionCallbackResult
    {
        public bool IsSuccess { get; set; }
        public bool IsAlreadyProcessed { get; set; }
        public int? TenantId { get; set; }
        public string? TenantSubdomain { get; set; }
        public int? SubscriptionId { get; set; }
        public string? ErrorMessage { get; set; }

        public static SubscriptionCallbackResult Success(int subscriptionId, int tenantId, string subdomain) =>
            new() { IsSuccess = true, SubscriptionId = subscriptionId, TenantId = tenantId, TenantSubdomain = subdomain };

        public static SubscriptionCallbackResult Failed(string? error = null) =>
            new() { IsSuccess = false, ErrorMessage = error };

        public static SubscriptionCallbackResult AlreadyProcessed(int subscriptionId, int tenantId, string subdomain) =>
            new() { IsSuccess = true, IsAlreadyProcessed = true, SubscriptionId = subscriptionId, TenantId = tenantId, TenantSubdomain = subdomain };
    }
}
