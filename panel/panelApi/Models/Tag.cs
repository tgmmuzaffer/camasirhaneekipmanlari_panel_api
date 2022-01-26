using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models
{
    public class Tag :IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
