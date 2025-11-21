using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public int? TemplateId { get; set; }  
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public Template? Template { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
