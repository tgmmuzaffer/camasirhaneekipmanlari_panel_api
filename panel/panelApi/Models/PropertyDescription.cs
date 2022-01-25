namespace panelApi.Models
{
    public class PropertyDescription : IEntity
    {
        public int Id { get; set; }
        public int ProductPropertyId { get; set; }
        public string Name { get; set; }
    }
}
