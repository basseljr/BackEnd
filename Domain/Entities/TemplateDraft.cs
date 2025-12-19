using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TemplateDraft
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int TemplateId { get; set; }
        public string CustomizationData { get; set; } = "{}";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;
        public int? UserId { get; set; }
        public User? User { get; set; }


    }
}
