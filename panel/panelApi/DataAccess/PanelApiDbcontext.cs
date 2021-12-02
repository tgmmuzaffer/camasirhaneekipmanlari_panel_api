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
        public virtual DbSet<PropertyTab> PropertyTabs { get; set; }
    }
}
