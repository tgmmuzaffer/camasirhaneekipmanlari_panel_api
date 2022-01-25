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
    public class CategoryRepo : ICategoryRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public CategoryRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Category> Create(Category entity)
        {
            _panelApiDbcontext.Categories.Add(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(Category entity)
        {
            _panelApiDbcontext.Categories.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<Category> Get(Expression<Func<Category, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.Categories.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Categories.FirstOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<Category>> GetList(Expression<Func<Category, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.Categories.Where(filter).ToListAsync() : await _panelApiDbcontext.Categories.ToListAsync();
            return result;
        }

        public async Task<bool> IsExist(Expression<Func<Category, bool>> filter = null)
        {
            var result = await _panelApiDbcontext.Categories.AnyAsync(filter);
            return result;
        }

        public async Task<bool> Update(Category entity)
        {
            _panelApiDbcontext.Categories.Update(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }
    }
}
