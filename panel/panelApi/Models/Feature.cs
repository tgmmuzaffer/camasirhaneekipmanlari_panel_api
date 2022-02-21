using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models
{
    public class Feature  : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public int SubCategoryId { get; set; }        
        public ICollection<SubCategory> SubCategories { get; set; }
        public ICollection<FeatureDescription> FeatureDescriptions { get; set; }
        [NotMapped]
        public ICollection<Fe_SubCat_Relational> Fe_SubCat_Relationals { get; set; }


    }
}
