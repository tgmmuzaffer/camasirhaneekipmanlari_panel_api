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
    public class ProductPropertyRepo : IProductPropertyRepo
    {
        private readonly ILogger<ProductPropertyRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ProductPropertyRepo(PanelApiDbcontext panelApiDbcontext, ILogger<ProductPropertyRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<ProductProperty> Create(ProductProperty entity)
        {
            try
            {
                _panelApiDbcontext.ProductProperties.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductPropertyRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(ProductProperty entity)
        {
            try
            {
                _panelApiDbcontext.ProductProperties.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductPropertyRepo Delete // {e.Message}");
                return false;
            }

        }

        public async Task<ProductProperty> Get(Expression<Func<ProductProperty, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.ProductProperties.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.ProductProperties.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductPropertyRepo Get // {e.Message}");
                return null;
            }

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
                _logger.LogError($"ProductPropertyRepo GetList // {e.Message}");
                return null;
            }

        }

        public async Task<List<string>> GetNames(Expression<Func<ProductProperty, bool>> filter)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.ProductProperties.Where(filter).Select(a => a.Name).ToListAsync() : await _panelApiDbcontext.ProductProperties.Select(a => a.Name).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductPropertyRepo GetList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<ProductProperty, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.ProductProperties.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductPropertyRepo IsExist // {e.Message}");
                return false;
            }

        }

        public async Task<bool> Update(ProductProperty entity)
        {
            try
            {
                _panelApiDbcontext.ProductProperties.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductPropertyRepo Update // {e.Message}");
                return false;
            }

        }
    }
}
