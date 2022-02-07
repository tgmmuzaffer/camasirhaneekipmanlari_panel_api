using System;

namespace panelApi.Models
{
    public class Product :IEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsPublish { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
