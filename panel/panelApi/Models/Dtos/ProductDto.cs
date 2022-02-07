using System;

namespace panelApi.Models.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string  ImageName { get; set; }
        public bool IsPublish { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
