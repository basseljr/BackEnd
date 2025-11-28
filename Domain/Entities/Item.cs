using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int StockQuantity { get; set; } = 0;
        public double DiscountPercentage { get; set; } = 0;
        public bool IsTrackStock { get; set; } = true;

        // Computed property (NOT stored in DB)
        public decimal FinalPrice => Price - (Price * (decimal)DiscountPercentage / 100m);

        public int? StockId { get; set; }
        public Stock? Stock { get; set; }

        public Tenant? Tenant { get; set; }
        public Category? Category { get; set; }
    }

}
