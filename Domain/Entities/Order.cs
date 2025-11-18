using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string Mode { get; set; } = "Pickup";
        public decimal Total { get; set; }
        public string Status { get; set; } = "Pending";

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
