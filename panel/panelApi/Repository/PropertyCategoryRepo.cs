using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PropertyCategoryRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public PropertyCategoryRepo(PanelApiDbcontext panelApiDbcontext, ILogger<PropertyCategoryRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<PropertyCategory> Create(PropertyCategory entity)
        {
            try
            {
                _panelApiDbcontext.PropertyCategories.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError("PropertyCategoryRepo Create", $"{e.Message}");
                return null;
            }

        }

        public async Task<bool> Delete(PropertyCategory entity)
        {
            try
            {
                _panelApiDbcontext.PropertyCategories.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("PropertyCategoryRepo Delete", $"{e.Message}");
                return false;
            }

        }

        public async Task<PropertyCategory> Get(Expression<Func<PropertyCategory, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.PropertyCategories.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.PropertyCategories.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("PropertyCategoryRepo Get", $"{e.Message}");
                return null;
            }

        }

        public async Task<List<int>> GetIdList(Expression<Func<PropertyCategory, bool>> filter)
        {
            try
            {
                var result = filter != null ?
                              await _panelApiDbcontext.PropertyCategories.Where(filter).Select(a => a.ProductPropertyId).ToListAsync() :
                              await _panelApiDbcontext.PropertyCategories.Select(a => a.ProductPropertyId).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("PropertyCategoryRepo GetIdList", $"{e.Message}");
                return null;
            }

        }

        public async Task<ICollection<PropertyCategory>> GetList(Expression<Func<PropertyCategory, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.PropertyCategories.Where(filter).ToListAsync() : await _panelApiDbcontext.PropertyCategories.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("PropertyCategoryRepo GetList", $"{e.Message}");
                return null;
            }

        }

        public async Task<bool> IsExist(Expression<Func<PropertyCategory, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.PropertyCategories.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("PropertyCategoryRepo IsExist", $"{e.Message}");
                return false;
            }

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
                _logger.LogError("PropertyCategoryRepo RemoveMultiple", $"{e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(PropertyCategory entity)
        {
            try
            {
                _panelApiDbcontext.PropertyCategories.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("PropertyCategoryRepo Update", $"{e.Message}");
                return false;
            }

        }
    }
}
