using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    public class Product : IEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [NotMapped]
        public SubCategory SubCategory { get; set; }
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public string ImageName { get; set; }
        public bool IsPublish { get; set; }
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public List<Feature> Feature { get; set; } = new List<Feature>();
        [NotMapped]
        public List<FeatureDescription> FeatureDescriptions { get; set; } = new List<FeatureDescription>();
        [NotMapped]
        public List<Pr_Fe_Relational> Pr_Fe_Relationals { get; set; } = new List<Pr_Fe_Relational>();
        [NotMapped]
        public List<Pr_FeDesc_Relational> Pr_FeDesc_Relationals { get; set; } = new List<Pr_FeDesc_Relational>();

    }

    public class ProductList
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Feature> Features { get; set; } = new List<Feature>();
    }
}
