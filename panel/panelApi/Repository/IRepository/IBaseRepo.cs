using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IBaseRepo<T> where T : class, new()
    {
        Task<ICollection<T>> GetList(Expression<Func<T, bool>> filter = null);
        Task<T> Get(Expression<Func<T, bool>> filter = null);
        Task<T> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> IsExist(Expression<Func<T, bool>> filter = null);
        //Task Save();
    }
}
