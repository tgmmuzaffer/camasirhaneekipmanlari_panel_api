using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace panelApi.Repository.IRepository
{
    public interface IBaseRepo<T> where T : class ,IEntity, new()
    {
        ICollection<T> GetList(Expression<Func<T, bool>> filter = null);
        T Get(Expression<Func<T, bool>> filter = null);
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
    }
}
