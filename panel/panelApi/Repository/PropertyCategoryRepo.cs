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
    public class PropertyCategoryRepo : IPropertyCategoryRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public PropertyCategoryRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<PropertyCategory> Create(PropertyCategory entity)
        {
            _panelApiDbcontext.PropertyCategories.Add(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(PropertyCategory entity)
        {
            _panelApiDbcontext.PropertyCategories.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<PropertyCategory> Get(Expression<Func<PropertyCategory, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.PropertyCategories.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.PropertyCategories.FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<int>> GetIdList(Expression<Func<PropertyCategory, bool>> filter)
        {
            var result = filter != null ? 
                await _panelApiDbcontext.PropertyCategories.Where(filter).Select(a => a.ProductPropertyId).ToListAsync() :
                await _panelApiDbcontext.PropertyCategories.Select(a => a.ProductPropertyId).ToListAsync();
            return result;
        }

        public async Task<ICollection<PropertyCategory>> GetList(Expression<Func<PropertyCategory, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.PropertyCategories.Where(filter).ToListAsync() : await _panelApiDbcontext.PropertyCategories.ToListAsync();
            return result;
        }

        public async Task<bool> IsExist(Expression<Func<PropertyCategory, bool>> filter = null)
        {
            var result = await _panelApiDbcontext.PropertyCategories.AnyAsync(filter);
            return result;
        }

        public async Task<bool> RemoveMultiple(int Id)
        {
            try
            {
                var list = await _panelApiDbcontext.PropertyCategories.Where(a => a.ProductPropertyId == Id).ToListAsync();
                _panelApiDbcontext.PropertyCategories.RemoveRange(list);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> Update(PropertyCategory entity)
        {
            _panelApiDbcontext.PropertyCategories.Update(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }
    }
}
