using Microsoft.EntityFrameworkCore;
using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.DataAccess
{
    public class LogDbcontext: DbContext
    {
        public LogDbcontext(DbContextOptions<LogDbcontext> opt) : base(opt)
        {
            Database.EnsureCreated();
        }
        public virtual DbSet<Log> Logs { get; set; }

    }
}
