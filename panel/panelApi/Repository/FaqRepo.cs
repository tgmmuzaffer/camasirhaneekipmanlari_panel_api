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
    public class FaqRepo : IFaqRepo
    {
        private readonly ILogger<FaqRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public FaqRepo(PanelApiDbcontext panelApiDbcontext, ILogger<FaqRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Faq> Create(Faq entity)
        {
            try
            {
                _panelApiDbcontext.Faqs.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"FaqRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(Faq entity)
        {
            try
            {
                _panelApiDbcontext.Faqs.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"FaqRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Faq> Get(Expression<Func<Faq, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Faqs.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Faqs.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FaqRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<List<Faq>> GetListWithRelatedEntity(Expression<Func<Faq, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Faqs.Where(filter).ToListAsync() : await _panelApiDbcontext.Faqs.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FaqRepo GetListWithRelatedEntity // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Faq, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Faqs.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FaqRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Faq entity)
        {
            try
            {
                _panelApiDbcontext.Faqs.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"FaqRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
