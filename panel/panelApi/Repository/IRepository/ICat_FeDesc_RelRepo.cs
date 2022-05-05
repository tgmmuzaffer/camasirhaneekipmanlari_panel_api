using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface ICat_FeDesc_RelRepo : IBaseRepo<Cat_FeDesc_Realational>
    {
        Task<bool> CreateMultiple(List<Cat_FeDesc_Realational> entity);
        Task<List<int>> GetFeatureDescIdList(Expression<Func<Cat_FeDesc_Realational, bool>> filter = null);
    }
}
