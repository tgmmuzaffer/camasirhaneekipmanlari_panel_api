using Microsoft.EntityFrameworkCore;
using panelApi.DataAccess;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository
{
    public class RoleRepo : IRoleRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public RoleRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<Role> GetRole(Expression<Func<Role, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _panelApiDbcontext.Roles.FirstOrDefaultAsync(filter);
            }
            return await _panelApiDbcontext.Roles.FirstOrDefaultAsync();
        }

        public async Task<ICollection<Role>> GetRoles()
        {
            return await _panelApiDbcontext.Roles.ToListAsync();
        }
    }
}
