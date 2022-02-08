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
    public class ProductRepo : IProductRepo
    {
        private readonly ILogger<ProductRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ProductRepo(PanelApiDbcontext panelApiDbcontext, ILogger<ProductRepo> logger)
        {
            _logger = logger;
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
                _logger.LogError($"ProductRepo Create // {e.Message}");
                return null;
            }

        }

        public async Task<bool> Delete(Product entity)
        {
            try
            {
                _panelApiDbcontext.Products.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo Delete // {e.Message}");
                return false;
            }

        }

        public async Task<Product> Get(Expression<Func<Product, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Products.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Products.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo Get // {e.Message}");
                return null;
            }

        }

        public async Task<ICollection<Product>> GetList(Expression<Func<Product, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Products.Where(filter).ToListAsync() : await _panelApiDbcontext.Products.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo GetList // {e.Message}");
                return null;
            }

        }

        public async Task<bool> IsExist(Expression<Func<Product, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Products.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo IsExist // {e.Message}");
                return false;
            }

        }

        public async Task<bool> Update(Product entity)
        {
            try
            {
                _panelApiDbcontext.Products.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo Update // {e.Message}");
                return false;
            }

        }
    }
}
