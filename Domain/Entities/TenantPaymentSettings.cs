using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TenantPaymentSettings
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int GatewayTypeId { get; set; }

        public string ApiKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string ExtraConfigJson { get; set; } = ""; 

        public Tenant Tenant { get; set; }
        public PaymentGatewayType GatewayType { get; set; }
    }

}
