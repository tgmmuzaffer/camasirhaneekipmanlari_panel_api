using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    public class Industry : IEntity
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public string ImageData { get; set; }
        public string Description { get; set; }
    }

    //public class Industries
    //{
    //    public int Id { get; set; }
    //    public List<Industry>IryListndust { get; set; }
    //}
}
