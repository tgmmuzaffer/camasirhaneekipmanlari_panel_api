using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models
{
    public class SubCategory : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Feature> Features { get; set; } = new List<Feature>();
        [NotMapped]
        public ICollection<Fe_SubCat_Relational> Fe_SubCat_Relationals { get; set; }

    }
}
