using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PreviewImage { get; set; } = string.Empty;
        public string DemoUrl { get; set; } = string.Empty;    
        public string Category { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? CustomizationData { get; set; }
        public string? Subdomain { get; set; }
        public bool IsPublished { get; set; } = false;
    }
}
