using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TenantPaymentSettingsDto
    {
        public int GatewayTypeId { get; set; }
        public string ApiKey { get; set; } = string.Empty;
        public string? SecretKey { get; set; }
    }
}
