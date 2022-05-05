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
    public class TagRepo : ITagRepo
    {
        private readonly ILogger<TagRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public TagRepo(PanelApiDbcontext panelApiDbcontext, ILogger<TagRepo> logger)
        {
            _logger = logger;
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
                _logger.LogError($"TagRepo Create // {e.Message}");
                return null;
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
                _logger.LogError($"TagRepo Delete // {e.Message}");
                return false;
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
                _logger.LogError($"TagRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<List<Tag>> GetListWithRelatedEntity(Expression<Func<Tag, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Tags.Where(filter).ToListAsync() : await _panelApiDbcontext.Tags.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"TagRepo GetListWithRelatedEntity // {e.Message}");
                return null;
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
                _logger.LogError($"TagRepo IsExist // {e.Message}");
                return false;
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
                _logger.LogError($"TagRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
