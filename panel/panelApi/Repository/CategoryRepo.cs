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
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ILogger<CategoryRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public CategoryRepo(PanelApiDbcontext panelApiDbcontext, ILogger<CategoryRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Category> Create(Category entity)
        {
            try
            {
                _panelApiDbcontext.Categories.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"CategoryRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(Category entity)
        {
            using var transaction = _panelApiDbcontext.Database.BeginTransaction();
            try
            {
                var catRels = await _panelApiDbcontext.Cat_Fe_Relatianals.AsNoTracking().Where(a => a.CategoryId == entity.Id).ToListAsync();
                if (catRels != null && catRels.Count > 0)
                {
                    _panelApiDbcontext.Cat_Fe_Relatianals.RemoveRange(catRels);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                _panelApiDbcontext.Categories.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"CategoryRepo Delete // {e.Message}");
                transaction.Rollback();
                return false;
            }
        }

        public async Task<Category> Get(Expression<Func<Category, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.Categories.Include(a => a.Products).Include(b => b.SubCategories).Where(filter).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync()
                    : await _panelApiDbcontext.Categories.Include(a => a.Products).Include(b => b.SubCategories).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"CategoryRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<Category> GetName(int Id)
        {
            try
            {
                var result = await _panelApiDbcontext.Categories.Where(a => a.Id == Id).AsNoTracking().FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"CategoryRepo GetName // {e.Message}");
                return null;
            }
        }

        public async Task<List<Category>> GetListWithRelatedEntity(Expression<Func<Category, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.Categories.Include(a => a.Products).Include(b => b.SubCategories).Where(filter).AsNoTracking().AsSplitQuery().ToListAsync()
                    : await _panelApiDbcontext.Categories.Include(a => a.Products).Include(b => b.SubCategories).AsNoTracking().AsSplitQuery().ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"CategoryRepo GetListWithRelatedEntity // {e.Message}");
                return null;
            }
        }

        public async Task<List<Category>> GetNameList()
        {
            try
            {
                var result = await _panelApiDbcontext.Categories.AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"CategoryRepo GetNameList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Category, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Categories.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"CategoryRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Category entity)
        {
            try
            {
                _panelApiDbcontext.Categories.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"CategoryRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
