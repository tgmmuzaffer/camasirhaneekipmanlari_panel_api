using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IProductPropertyRepo : IBaseRepo<ProductProperty>
    {
        Task<List<string>> GetNames(Expression<Func<ProductProperty, bool>> filter);
    }
}
