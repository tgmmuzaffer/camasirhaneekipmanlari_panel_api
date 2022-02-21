using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models
{
    public class Pr_Fe_Relational : IEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public Product Product{ get; set; }
        public int FeatureId { get; set; }
        [NotMapped]
        public Feature Feature{ get; set; }
    }
}
