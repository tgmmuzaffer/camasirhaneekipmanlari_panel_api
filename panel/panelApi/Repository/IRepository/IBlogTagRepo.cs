using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IBlogTagRepo : IBaseRepo<BlogTag>
    {
        Task<List<BlogTag>> AddList(List<BlogTag> entity);
        Task<List<int>> GetIdList(Expression<Func<BlogTag, bool>> filter);
        Task<bool> RemoveMultiple(int Id);
    }
}
