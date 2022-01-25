using Microsoft.EntityFrameworkCore;
using panelApi.DataAccess;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository
{
    public class ProductRepo : IProductRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ProductRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<Product> Create(Product entity)
        {
            try
            {
                _panelApiDbcontext.Products.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<bool> Delete(Product entity)
        {
            _panelApiDbcontext.Products.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<Product> Get(Expression<Func<Product, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.Products.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Products.FirstOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<Product>> GetList(Expression<Func<Product, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.Products.Where(filter).ToListAsync() : await _panelApiDbcontext.Products.ToListAsync();
            return result;
        }

        public async Task<bool> IsExist(Expression<Func<Product, bool>> filter = null)
        {
            var result = await _panelApiDbcontext.Products.AnyAsync(filter);
            return result;
        }

        public async Task<bool> Update(Product entity)
        {
            _panelApiDbcontext.Products.Update(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }
    }
}
