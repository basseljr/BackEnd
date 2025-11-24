using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SalesSummaryDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Values { get; set; } = new();
        public string Period { get; set; } = "daily";
    }
}
