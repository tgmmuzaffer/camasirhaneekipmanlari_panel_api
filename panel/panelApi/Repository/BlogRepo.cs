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
    public class BlogRepo : IBlogRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        private readonly ILogger<BlogRepo> _logger;

        public BlogRepo(PanelApiDbcontext panelApiDbcontext, ILogger<BlogRepo> logger)
        {
            _panelApiDbcontext = panelApiDbcontext;
            _logger = logger;
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
                _logger.LogError($"BlogRepo Create // {e.Message}");
                return null;
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
                _logger.LogError($"BlogRepo Delete // {e.Message}");
                return false;
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
                _logger.LogError($"BlogRepo Get // {e.Message}");
                return null;
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
                _logger.LogError($"BlogRepo GetList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Blog, bool>> filter = null)
        {
            try
            {
                return await _panelApiDbcontext.Blogs.AnyAsync(filter);
            }
            catch (Exception e)
            {
                _logger.LogError($"BlogRepo IsExist // {e.Message}");
                return false;
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
                _logger.LogError($"BlogRepo Update // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Save()
        {
            try
            {
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"BlogRepo Save // {e.Message}");
                return false;
            }
        }

        
    }
}