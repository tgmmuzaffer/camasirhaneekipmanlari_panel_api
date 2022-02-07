namespace panelApi.Models
{
    public class Referance : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
    }
}
