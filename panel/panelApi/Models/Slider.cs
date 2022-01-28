using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models
{
    public class Slider :IEntity
    {
        public int Id { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }
        public string Link { get; set; }
        public string ButtonName { get; set; }
        public string ImagePath { get; set; }
    }
}
