using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SaveDraftDto
    {

        public string Email { get; set; }
        public int TemplateId { get; set; }
        public string CustomizationData { get; set; } = "{}";
    }
}
