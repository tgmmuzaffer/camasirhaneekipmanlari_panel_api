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
        public virtual DbSet<ProductProperty> ProductProperties { get; set; }
        public virtual DbSet<PropertyDescription> PropertyDescriptions { get; set; }
        public virtual DbSet<PropertyCategory> PropertyCategories { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<BlogTag> BlogTags { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
    }
}
