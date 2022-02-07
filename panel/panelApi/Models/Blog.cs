using System;

namespace panelApi.Models
{
    public class Blog : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
