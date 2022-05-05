using panelApi.Helpers;
using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IProductRepo : IBaseRepo<Product>
    {
        Task<List<Product>> GetPagedList(Expression<Func<Product, bool>> filter = null);
        //Task<PagedList<Product>> GetPagedListSql(int Id, ProductParameters productParameters = null);
        Task<List<Product>> GetList(Expression<Func<Product, bool>> filter = null);

    }
}
