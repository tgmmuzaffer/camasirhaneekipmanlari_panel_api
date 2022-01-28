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
    public class TagRepo : ITagRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public TagRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Tag> Create(Tag entity)
        {
            try
            {
                _panelApiDbcontext.Tags.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> Delete(Tag entity)
        {
            try
            {
                _panelApiDbcontext.Tags.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Tag> Get(Expression<Func<Tag, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Tags.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Tags.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<ICollection<Tag>> GetList(Expression<Func<Tag, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Tags.Where(filter).ToListAsync() : await _panelApiDbcontext.Tags.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> IsExist(Expression<Func<Tag, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Tags.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> Update(Tag entity)
        {
            try
            {
                _panelApiDbcontext.Tags.Update(entity);
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
