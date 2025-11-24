using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TenantCustomization
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int TemplateId { get; set; } // which template they chose
        public string CustomizationData { get; set; } = "{}"; // JSON string
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public Tenant? Tenant { get; set; }
        public Template? Template { get; set; }
    }

}
