using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Stock
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; } = "Unit"; // or Kg, L, etc.
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public Tenant? Tenant { get; set; }
        public Item? Item { get; set; }
    }

}
