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
    public class Pr_FeDesc_RelRepo : IPr_FeDesc_RelRepo
    {

        private readonly ILogger<Pr_FeDesc_RelRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public Pr_FeDesc_RelRepo(PanelApiDbcontext panelApiDbcontext, ILogger<Pr_FeDesc_RelRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Pr_FeDesc_Relational> Create(Pr_FeDesc_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Pr_FeDesc_Relationals.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_FeDesc_RelRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> CreateMultiple(List<Pr_FeDesc_Relational> entity)
        {
            try
            {
                _panelApiDbcontext.Pr_FeDesc_Relationals.AddRange(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_FeDesc_RelRepo Create // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(Pr_FeDesc_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Pr_FeDesc_Relationals.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_FeDesc_RelRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Pr_FeDesc_Relational> Get(Expression<Func<Pr_FeDesc_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Pr_FeDesc_Relationals.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Pr_FeDesc_Relationals.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_FeDesc_RelRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<ICollection<Pr_FeDesc_Relational>> GetList(Expression<Func<Pr_FeDesc_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.Pr_FeDesc_Relationals.Where(filter).AsNoTracking().ToListAsync()
                    : await _panelApiDbcontext.Pr_FeDesc_Relationals.AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_FeDesc_RelRepo GetList // {e.Message}");
                return null;
            }
        }
        public async Task<ICollection<int>> GetFeatureDescIdList(Expression<Func<Pr_FeDesc_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.Pr_FeDesc_Relationals
                    .Where(filter)
                    .AsNoTracking()
                    .Select(a => a.FeatureDescriptionId)
                    .ToListAsync()
                    : await _panelApiDbcontext.Pr_FeDesc_Relationals
                    .AsNoTracking()
                    .Select(a => a.FeatureDescriptionId)
                    .ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_FeDesc_RelRepo GetFeatureDescIdList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Pr_FeDesc_Relational, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Pr_FeDesc_Relationals.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_FeDesc_RelRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Pr_FeDesc_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Pr_FeDesc_Relationals.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_FeDesc_RelRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
