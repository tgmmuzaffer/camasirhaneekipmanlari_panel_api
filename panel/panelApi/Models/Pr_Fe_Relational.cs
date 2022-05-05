using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    public class Pr_Fe_Relational : IEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public Product Product { get; set; }
        public int FeatureId { get; set; }
        [NotMapped]
        public Feature Feature { get; set; }
    }
}
