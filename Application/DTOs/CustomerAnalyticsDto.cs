using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CustomerAnalyticsDto
    {
        public int NewCustomers { get; set; }
        public int ReturningCustomers { get; set; }
        public int TotalCustomers { get; set; }
    }
}
