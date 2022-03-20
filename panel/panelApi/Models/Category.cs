using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    public class Category : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public string ImageName { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}
