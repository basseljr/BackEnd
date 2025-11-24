using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DiscountItem
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public int ItemId { get; set; }

        public Discount? Discount { get; set; }
        public Item? Item { get; set; }
    }
}
