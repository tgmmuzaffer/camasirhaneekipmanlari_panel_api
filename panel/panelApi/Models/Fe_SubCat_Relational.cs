namespace panelApi.Models
{
    public class Fe_SubCat_Relational
    {
        public int Id { get; set; }
        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        public int FeatureId { get; set; }
        public Feature Feature { get; set; }

    }
}
