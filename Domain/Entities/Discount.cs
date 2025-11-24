using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = "Percent"; // Percent or Fixed
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? UsageLimit { get; set; }
        public bool AppliesToAllItems { get; set; } = true; // true = all items

        // If false, link specific items or categories
        public ICollection<DiscountItem> DiscountItems { get; set; } = new List<DiscountItem>();
        public ICollection<DiscountCategory> DiscountCategories { get; set; } = new List<DiscountCategory>();

        public Tenant? Tenant { get; set; }
    }


}
