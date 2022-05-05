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
    public class SubCategoryRepo : ISubCategoryRepo
    {
        private readonly ILogger<SubCategoryRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public SubCategoryRepo(PanelApiDbcontext panelApiDbcontext, ILogger<SubCategoryRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<SubCategory> Create(SubCategory entity)
        {
            try
            {
                _panelApiDbcontext.SubCategories.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"SubCategoryRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(SubCategory entity)
        {
            try
            {
                _panelApiDbcontext.SubCategories.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"SubCategoryRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<SubCategory> Get(Expression<Func<SubCategory, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.SubCategories
                    .Include(a => a.Category)
                    .Include(b => b.Features)
                    .Where(filter)
                    .OrderBy(a => a.Name)
                    .FirstOrDefaultAsync() :
                    await _panelApiDbcontext.SubCategories
                    .Include(a => a.Category)
                    .Include(b => b.Features)
                    .OrderBy(a => a.Name)
                    .FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"SubCategoryRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<List<SubCategory>> GetListWithRelatedEntity(Expression<Func<SubCategory, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.SubCategories
                    .Include(a => a.Category)
                    .Where(filter)
                    .AsNoTracking().ToListAsync()
                    : await _panelApiDbcontext.SubCategories
                    .Include(a => a.Category)
                    .AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"SubCategoryRepo GetListWithRelatedEntity // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<SubCategory, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.SubCategories.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"SubCategoryRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(SubCategory entity)
        {
            try
            {
                _panelApiDbcontext.SubCategories.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"SubCategoryRepo Update // {e.Message}");
                return false;
            }
        }

        public async Task<int> GetCategoryId(Expression<Func<SubCategory, bool>> filter = null)
        {
            try
            {
                int result = filter == null ?
                      await _panelApiDbcontext.SubCategories.Where(filter).AsNoTracking().Select(a => a.CategoryId).FirstOrDefaultAsync() :
                      await _panelApiDbcontext.SubCategories.AsNoTracking().Select(a => a.CategoryId).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"SubCategoryRepo Update // {e.Message}");
                return 0;
            }
        }
    }
}
