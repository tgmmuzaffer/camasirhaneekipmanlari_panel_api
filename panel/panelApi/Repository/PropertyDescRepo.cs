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
    public class PropertyDescRepo : IPropertyDesRepo
    {
        private readonly ILogger<PropertyDescRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public PropertyDescRepo(PanelApiDbcontext panelApiDbcontext, ILogger<PropertyDescRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<PropertyDescription> Create(PropertyDescription entity)
        {
            try
            {
                _panelApiDbcontext.PropertyDescriptions.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"PropertyCategoryRepo Create // {e.Message}");
                return null;
            }

        }

        public async Task<bool> Delete(PropertyDescription entity)
        {
            try
            {
                _panelApiDbcontext.PropertyDescriptions.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"PropertyCategoryRepo Delete // {e.Message}");
                return false;
            }

        }

        public async Task<PropertyDescription> Get(Expression<Func<PropertyDescription, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.PropertyDescriptions.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.PropertyDescriptions.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"PropertyCategoryRepo Get // {e.Message}");
                return null;
            }

        }

        public async Task<ICollection<PropertyDescription>> GetList(Expression<Func<PropertyDescription, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.PropertyDescriptions.Where(filter).ToListAsync() : await _panelApiDbcontext.PropertyDescriptions.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"PropertyCategoryRepo GetList // {e.Message}");
                return null;
            }

        }

        public async Task<bool> IsExist(Expression<Func<PropertyDescription, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.PropertyDescriptions.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"PropertyCategoryRepo IsExist // {e.Message}");
                return false;
            }

        }

        public async Task<bool> Update(PropertyDescription entity)
        {
            try
            {
                _panelApiDbcontext.PropertyDescriptions.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"PropertyCategoryRepo Update // {e.Message}");
                return false;
            }

        }
    }
}
