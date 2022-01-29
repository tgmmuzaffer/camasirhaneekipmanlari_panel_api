using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models.Dtos
{
    public class ReferanceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string ImageData { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
    }
}
