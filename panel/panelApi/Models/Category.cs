using System.Collections.Generic;

namespace panelApi.Models
{
    public class Category : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }
    }
}
