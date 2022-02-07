namespace panelApi.Models
{
    public class Contact : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string MapAdress { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string WorkHour1 { get; set; }
        public string WorkHour2 { get; set; }
    }
}
