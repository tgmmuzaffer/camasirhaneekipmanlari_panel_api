using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    public class Cat_Fe_Relational : IEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [NotMapped]
        public Category Category { get; set; }
        public int FeatureId { get; set; }
        [NotMapped]
        public Feature Feature { get; set; }
    }
}
