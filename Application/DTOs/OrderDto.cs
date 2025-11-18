using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
