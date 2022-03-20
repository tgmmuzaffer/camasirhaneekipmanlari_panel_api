using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Models
{
    public class AboutUs : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }
        public string ImagePath1 { get; set; }
        [NotMapped]
        public string ImageName1 { get; set; }
        public string ImagePath2 { get; set; }
        [NotMapped]
        public string ImageName2 { get; set; }
        public string ImagePath3 { get; set; }
        [NotMapped]
        public string ImageName3 { get; set; }
    }
}
