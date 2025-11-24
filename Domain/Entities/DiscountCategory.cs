using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DiscountCategory
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public int CategoryId { get; set; }

        public Discount? Discount { get; set; }
        public Category? Category { get; set; }
    }
}
