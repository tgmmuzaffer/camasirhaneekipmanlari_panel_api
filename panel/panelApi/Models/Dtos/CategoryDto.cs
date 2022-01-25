using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductId { get; set; }
        public List<int> ProductPropertyIds { get; set; }
        public List<string> ProductPropertyNames { get; set; } = new List<string>();
    }
}
