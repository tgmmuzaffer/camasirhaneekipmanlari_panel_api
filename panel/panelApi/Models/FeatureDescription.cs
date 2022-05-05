namespace panelApi.Models
{
    public class FeatureDescription
    {
        public int Id { get; set; }
        public string FeatureDesc { get; set; }
        public int FeatureId { get; set; }
        public Feature Feature { get; set; }
    }
}
