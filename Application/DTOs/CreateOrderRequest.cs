using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string Mode { get; set; } = "Pickup";
        public decimal Total { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
