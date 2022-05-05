using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    public class Cat_FeDesc_Realational : IEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [NotMapped]
        public Category Category { get; set; }
        public int FeatureDescriptionId { get; set; }
        [NotMapped]
        public FeatureDescription FeatureDescription { get; set; }
    }
}
