using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    public class SubCategory : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Feature> Features { get; set; } = new List<Feature>();
        [NotMapped]
        public List<Fe_SubCat_Relational> Fe_SubCat_Relationals { get; set; } = new List<Fe_SubCat_Relational>();

    }
}
