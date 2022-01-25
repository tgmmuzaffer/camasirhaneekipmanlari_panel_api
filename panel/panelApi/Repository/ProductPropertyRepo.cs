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
    public class ProductPropertyRepo : IProductPropertyRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ProductPropertyRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<ProductProperty> Create(ProductProperty entity)
        {
            _panelApiDbcontext.ProductProperties.Add(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(ProductProperty entity)
        {
            _panelApiDbcontext.ProductProperties.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<ProductProperty> Get(Expression<Func<ProductProperty, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.ProductProperties.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.ProductProperties.FirstOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<ProductProperty>> GetList(Expression<Func<ProductProperty, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.ProductProperties.Where(filter).ToListAsync() : await _panelApiDbcontext.ProductProperties.ToListAsync();
                return result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<List<string>> GetNames(Expression<Func<ProductProperty, bool>> filter)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.ProductProperties.Where(filter).Select(a=>a.Name).ToListAsync() : await _panelApiDbcontext.ProductProperties.Select(a => a.Name).ToListAsync();
                return result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<bool> IsExist(Expression<Func<ProductProperty, bool>> filter = null)
        {
            var result = await _panelApiDbcontext.ProductProperties.AnyAsync(filter);
            return result;
        }

        public async Task<bool> Update(ProductProperty entity)
        {
            _panelApiDbcontext.ProductProperties.Update(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }
    }
}
