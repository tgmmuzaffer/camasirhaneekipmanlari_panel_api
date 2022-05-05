using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IPr_FeDesc_RelRepo : IBaseRepo<Pr_FeDesc_Relational>
    {
        Task<bool> CreateMultiple(List<Pr_FeDesc_Relational> entity);
        Task<List<int>> GetFeatureDescIdList(Expression<Func<Pr_FeDesc_Relational, bool>> filter = null);
    }
}
