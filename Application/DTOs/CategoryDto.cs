using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CategoryOrderDto
    {
        public int Id { get; set; }
        public int DisplayOrder { get; set; }
    }
}
