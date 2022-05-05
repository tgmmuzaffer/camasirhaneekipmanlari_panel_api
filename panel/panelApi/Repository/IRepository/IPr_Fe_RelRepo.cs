using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IPr_Fe_RelRepo : IBaseRepo<Pr_Fe_Relational>
    {
        Task<bool> CreateMultiple(List<Pr_Fe_Relational> entity);
        Task<List<int>> GetFetureIdList(Expression<Func<Pr_Fe_Relational, bool>> filter = null);
    }
}
