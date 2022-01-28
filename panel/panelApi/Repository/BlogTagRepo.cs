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
    public class BlogTagRepo : IBlogTagRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public BlogTagRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<List<BlogTag>> AddList(List<BlogTag> entity)
        {
            try
            {
                foreach (var item in entity)
                {
                    _panelApiDbcontext.BlogTags.Add(item);
                    await _panelApiDbcontext.SaveChangesAsync();
                }
                return entity;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<BlogTag> Create(BlogTag entity)
        {
            try
            {
                _panelApiDbcontext.BlogTags.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> Delete(BlogTag entity)
        {
            try
            {
                _panelApiDbcontext.BlogTags.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<BlogTag> Get(Expression<Func<BlogTag, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.BlogTags.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.BlogTags.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<int>> GetIdList(Expression<Func<BlogTag, bool>> filter)
        {
            try
            {
                var result = filter != null ?
               await _panelApiDbcontext.BlogTags.Where(filter).Select(a => a.TagId).ToListAsync() :
               await _panelApiDbcontext.BlogTags.Select(a => a.TagId).ToListAsync();
                return result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<ICollection<BlogTag>> GetList(Expression<Func<BlogTag, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.BlogTags.Where(filter).ToListAsync() : await _panelApiDbcontext.BlogTags.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> IsExist(Expression<Func<BlogTag, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.BlogTags.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> RemoveMultiple(int Id)
        {
            try
            {
                var list = await _panelApiDbcontext.BlogTags.Where(a => a.BlogId == Id).ToListAsync();
                _panelApiDbcontext.BlogTags.RemoveRange(list);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> Update(BlogTag entity)
        {
            try
            {
                _panelApiDbcontext.BlogTags.Update(entity);
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
