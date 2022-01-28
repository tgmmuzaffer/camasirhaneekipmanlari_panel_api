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
    public class BlogRepo : IBlogRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public BlogRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Blog> Create(Blog entity)
        {
            try
            {
                _panelApiDbcontext.Blogs.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> Delete(Blog entity)
        {
            try
            {
                _panelApiDbcontext.Blogs.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Blog> Get(Expression<Func<Blog, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Blogs.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Blogs.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ICollection<Blog>> GetList(Expression<Func<Blog, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Blogs.Where(filter).ToListAsync() : await _panelApiDbcontext.Blogs.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> IsExist(Expression<Func<Blog, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Blogs.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<bool> Update(Blog entity)
        {
            try
            {
                _panelApiDbcontext.Blogs.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
