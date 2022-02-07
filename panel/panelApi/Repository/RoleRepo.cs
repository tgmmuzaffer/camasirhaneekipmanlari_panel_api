using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using panelApi.DataAccess;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository
{
    public class RoleRepo : IRoleRepo
    {
        private readonly ILogger<RoleRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public RoleRepo(PanelApiDbcontext panelApiDbcontext, ILogger<RoleRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<Role> GetRole(Expression<Func<Role, bool>> filter = null)
        {
            try
            {
                if (filter != null)
                {
                    return await _panelApiDbcontext.Roles.FirstOrDefaultAsync(filter);
                }

                return await _panelApiDbcontext.Roles.FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("RoleRepo GetRole", $"{e.Message}");
                return null;
            }

        }

        public async Task<ICollection<Role>> GetRoles()
        {
            try
            {
                return await _panelApiDbcontext.Roles.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("RoleRepo GetRoles", $"{e.Message}");
                return null;
            }
        }
    }
}
