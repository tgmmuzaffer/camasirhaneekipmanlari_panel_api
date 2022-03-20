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
        public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        public List<FeatureDescription> FeatureDescriptions { get; set; } = new List<FeatureDescription>();
        [NotMapped]
        public List<Fe_SubCat_Relational> Fe_SubCat_Relationals { get; set; } = new List<Fe_SubCat_Relational>();


    }
}
