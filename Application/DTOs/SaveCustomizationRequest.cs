using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SaveCustomizationRequest
    {
        public string Slug { get; set; } = string.Empty;
        public string? CustomizationData { get; set; }
    }

}
