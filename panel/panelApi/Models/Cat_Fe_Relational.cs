using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models
{
    public class Cat_Fe_Relational : IEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [NotMapped]
        public Category Category{ get; set; }
        public int FeatureId { get; set; }
        [NotMapped]
        public Feature Feature { get; set; }
    }
}
