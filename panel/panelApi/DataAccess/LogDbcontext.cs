using Microsoft.EntityFrameworkCore;
using panelApi.Models;

namespace panelApi.DataAccess
{
    public class LogDbcontext : DbContext
    {
        public LogDbcontext(DbContextOptions<LogDbcontext> opt) : base(opt)
        {
            Database.EnsureCreated();
        }
        public virtual DbSet<Log> Logs { get; set; }

    }
}
