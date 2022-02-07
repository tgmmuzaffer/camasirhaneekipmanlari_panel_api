namespace panelApi.Models
{
    public class BlogTag : IEntity
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public int TagId { get; set; }
    }
}
