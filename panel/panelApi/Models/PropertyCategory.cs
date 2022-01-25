using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models
{
    public class PropertyCategory : IEntity
    {
        public int Id { get; set; }
        public int CategoryId{ get; set; }
        public int ProductPropertyId { get; set; }

    }
}
