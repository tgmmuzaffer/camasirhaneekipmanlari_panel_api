using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface ICat_Fe_RelRepo : IBaseRepo<Cat_Fe_Relational>
    {
        Task<bool> UpdateCreate(List<Cat_Fe_Relational> entity);
        Task<bool> RemoveMultiple(ICollection<Cat_Fe_Relational> entity);
        Task<bool> IsPairExist(int featureId, int categoryId);
    }
}
