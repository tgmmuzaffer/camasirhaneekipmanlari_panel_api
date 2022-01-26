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
            _panelApiDbcontext.Tags.Add(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(Tag entity)
        {
            _panelApiDbcontext.Tags.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<Tag> Get(Expression<Func<Tag, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.Tags.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Tags.FirstOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<Tag>> GetList(Expression<Func<Tag, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.Tags.Where(filter).ToListAsync() : await _panelApiDbcontext.Tags.ToListAsync();
            return result;
        }

        public async Task<bool> IsExist(Expression<Func<Tag, bool>> filter = null)
        {
            var result = await _panelApiDbcontext.Tags.AnyAsync(filter);
            return result;
        }

        public async Task<bool> Update(Tag entity)
        {
            _panelApiDbcontext.Tags.Update(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }
    }
}
