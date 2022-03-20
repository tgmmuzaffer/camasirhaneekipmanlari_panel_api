using Microsoft.EntityFrameworkCore;
using panelApi.Models;

namespace panelApi.DataAccess
{
    public class PanelApiDbcontext:DbContext
    {
        public PanelApiDbcontext(DbContextOptions<PanelApiDbcontext>opt):base(opt)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<FeatureDescription> FeatureDescriptions { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; } 
        public virtual DbSet<BlogTag> BlogTags { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
        public virtual DbSet<Referance> Referances{ get; set; }
        public virtual DbSet<Pr_Fe_Relational> Pr_Fe_Relationals{ get; set; }
        public virtual DbSet<Pr_FeDesc_Relational> Pr_FeDesc_Relationals{ get; set; }
        public virtual DbSet<Fe_SubCat_Relational> Fe_SubCat_Relationals{ get; set; }
        public virtual DbSet<AboutUs> AboutUs{ get; set; }
        public virtual DbSet<Cat_FeDesc_Realational> Cat_FeDesc_Realationals{ get; set; }
        public virtual DbSet<Cat_Fe_Relational> Cat_Fe_Relatianals { get; set; }
    }
}
