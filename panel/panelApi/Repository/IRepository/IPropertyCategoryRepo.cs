using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IPropertyCategoryRepo: IBaseRepo<PropertyCategory>
    {
        Task<List<int>> GetIdList(Expression<Func<PropertyCategory, bool>> filter);
        Task<bool> RemoveMultiple(int Id);
    }
}
