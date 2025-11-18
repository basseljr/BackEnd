using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        // ✅ add this FK link to the menu item table
        public int? ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // ✅ optional navigation for richer includes later
        public Item? Item { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
