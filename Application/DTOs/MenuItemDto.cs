using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int StockQuantity { get; set; }
        public double DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public bool IsTrackStock { get; set; }
    }
}
