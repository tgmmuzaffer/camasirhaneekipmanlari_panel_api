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
    public class FeatureDescriptionRepo : IFeatureDescriptionRepo
    {
        private readonly ILogger<FeatureDescriptionRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public FeatureDescriptionRepo(PanelApiDbcontext panelApiDbcontext, ILogger<FeatureDescriptionRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<FeatureDescription> Create(FeatureDescription entity)
        {
            try
            {
                _panelApiDbcontext.FeatureDescriptions.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureDescriptionRepo Create // {e.Message}");
                return null;
            }

        }

        public async Task<bool> Delete(FeatureDescription entity)
        {
            try
            {
                _panelApiDbcontext.FeatureDescriptions.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureDescriptionRepo Delete // {e.Message}");
                return false;
            }

        }

        public async Task<FeatureDescription> Get(Expression<Func<FeatureDescription, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.FeatureDescriptions
                    .Include(a => a.Feature)
                    .Where(filter)
                    .OrderBy(a => a.FeatureDesc)
                    .FirstOrDefaultAsync() :
                    await _panelApiDbcontext.FeatureDescriptions
                    .Include(a => a.Feature)
                    .OrderBy(a => a.FeatureDesc)
                    .FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureDescriptionRepo Get // {e.Message}");
                return null;
            }

        }

        public async Task<ICollection<FeatureDescription>> GetList(Expression<Func<FeatureDescription, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.FeatureDescriptions.Include(a => a.Feature).Where(filter).ToListAsync() : await _panelApiDbcontext.FeatureDescriptions.Include(a => a.Feature).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureDescriptionRepo GetList // {e.Message}");
                return null;
            }

        }

        public async Task<bool> IsExist(Expression<Func<FeatureDescription, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.FeatureDescriptions.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureDescriptionRepo IsExist // {e.Message}");
                return false;
            }

        }

        public async Task<bool> Update(FeatureDescription entity)
        {
            try
            {
                _panelApiDbcontext.FeatureDescriptions.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureDescriptionRepo Update // {e.Message}");
                return false;
            }

        }
    }
}
