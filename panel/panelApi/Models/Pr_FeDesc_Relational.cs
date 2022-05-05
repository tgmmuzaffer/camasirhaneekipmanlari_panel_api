using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    //Fe_FeDesc_Relational
    public class Pr_FeDesc_Relational : IEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public Product Product { get; set; }
        public int FeatureDescriptionId { get; set; }
        [NotMapped]
        public FeatureDescription FeatureDescription { get; set; }
    }
}
