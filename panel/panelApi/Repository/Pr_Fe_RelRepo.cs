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
    public class Pr_Fe_RelRepo : IPr_Fe_RelRepo
    {
        private readonly ILogger<Pr_Fe_RelRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public Pr_Fe_RelRepo(PanelApiDbcontext panelApiDbcontext, ILogger<Pr_Fe_RelRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Pr_Fe_Relational> Create(Pr_Fe_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Pr_Fe_Relationals.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_Fe_Relational Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> CreateMultiple(List<Pr_Fe_Relational> entity)
        {
            try
            {
                _panelApiDbcontext.Pr_Fe_Relationals.AddRange(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_Fe_Relational CreateMultiple // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(Pr_Fe_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Pr_Fe_Relationals.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_Fe_Relational Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Pr_Fe_Relational> Get(Expression<Func<Pr_Fe_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Pr_Fe_Relationals.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Pr_Fe_Relationals.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_Fe_Relational Get // {e.Message}");
                return null;
            }
        }

        public async Task<List<Pr_Fe_Relational>> GetList(Expression<Func<Pr_Fe_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.Pr_Fe_Relationals.Where(filter).AsNoTracking().ToListAsync()
                    : await _panelApiDbcontext.Pr_Fe_Relationals.AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_Fe_Relational GetList // {e.Message}");
                return null;
            }
        }
         public async Task<List<int>> GetFetureIdList(Expression<Func<Pr_Fe_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.Pr_Fe_Relationals
                    .Where(filter)
                    .AsNoTracking()
                    .Select(a=>a.FeatureId).ToListAsync()
                    : await _panelApiDbcontext.Pr_Fe_Relationals
                    .AsNoTracking()
                    .Select(a => a.FeatureId)
                    .ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_Fe_Relational GetFetureIdList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Pr_Fe_Relational, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Pr_Fe_Relationals.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_Fe_Relational IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Pr_Fe_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Pr_Fe_Relationals.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Pr_Fe_Relational Update // {e.Message}");
                return false;
            }
        }
    }
}
