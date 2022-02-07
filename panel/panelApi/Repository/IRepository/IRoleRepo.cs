using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IRoleRepo
    {
        Task<ICollection<Role>> GetRoles();
        Task<Role> GetRole(Expression<Func<Role, bool>> filter = null);
    }
}
