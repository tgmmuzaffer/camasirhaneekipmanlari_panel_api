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
    public class IndustryRepo : IIndustryRepo
    {
        private readonly ILogger<SliderRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public IndustryRepo(PanelApiDbcontext panelApiDbcontext, ILogger<SliderRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Industry> Create(Industry entity)
        {
            try
            {
                _panelApiDbcontext.Industries.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"IndustryRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(Industry entity)
        {
            try
            {
                _panelApiDbcontext.Industries.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"IndustryRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Industry> Get(Expression<Func<Industry, bool>> filter = null)
        {
            try
            {
                var industry = await _panelApiDbcontext.Industries.FirstOrDefaultAsync(filter);
                return industry;
            }
            catch (Exception e)
            {
                _logger.LogError($"IndustryRepo Delete // {e.Message}");
                return null;
            }
        }

        public async Task<List<Industry>> GetListWithRelatedEntity(Expression<Func<Industry, bool>> filter = null)
        {
            try
            {
                var list = filter== null ? await _panelApiDbcontext.Industries.ToListAsync() : await _panelApiDbcontext.Industries.Where(filter).ToListAsync();
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"IndustryRepo GetList // {e.Message}");
                return null;
            }


        }
        public async Task<bool> IsExist(Expression<Func<Industry, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Industries.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"IndustryRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Industry entity)
        {
            try
            {
                _panelApiDbcontext.Industries.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"IndustryRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
