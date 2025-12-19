using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Relationships
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<User> Users { get; set; } = new List<User>(); // if staff
        public TenantCustomization? Customization { get; set; }
        public int? TemplateId { get; set; } 
        public bool IsPublished { get; set; } = false;
    }

}
