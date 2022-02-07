using System;
using System.Collections.Generic;

namespace panelApi.Models.Dtos
{
    public class BlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public DateTime CreateDate { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
        public List<string> TagNames { get; set; } = new List<string>();
    }
}
