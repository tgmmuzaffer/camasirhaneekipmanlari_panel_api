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
    public class Cat_FeDesc_RelRepo : ICat_FeDesc_RelRepo
    {
        private readonly ILogger<Cat_FeDesc_Realational> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public Cat_FeDesc_RelRepo(PanelApiDbcontext panelApiDbcontext, ILogger<Cat_FeDesc_Realational> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Cat_FeDesc_Realational> Create(Cat_FeDesc_Realational entity)
        {
            try
            {
                _panelApiDbcontext.Cat_FeDesc_Realationals.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_FeDesc_RelRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> CreateMultiple(List<Cat_FeDesc_Realational> entity)
        {
            try
            {
                _panelApiDbcontext.Cat_FeDesc_Realationals.AddRange(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_FeDesc_RelRepo Create // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(Cat_FeDesc_Realational entity)
        {
            try
            {
                _panelApiDbcontext.Cat_FeDesc_Realationals.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_FeDesc_RelRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Cat_FeDesc_Realational> Get(Expression<Func<Cat_FeDesc_Realational, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Cat_FeDesc_Realationals.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Cat_FeDesc_Realationals.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_FeDesc_RelRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<List<int>> GetFeatureDescIdList(Expression<Func<Cat_FeDesc_Realational, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.Cat_FeDesc_Realationals
                    .Where(filter)
                    .AsNoTracking()
                    .Select(a => a.FeatureDescriptionId)
                    .ToListAsync()
                    : await _panelApiDbcontext.Cat_FeDesc_Realationals
                    .AsNoTracking()
                    .Select(a => a.FeatureDescriptionId)
                    .ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_FeDesc_RelRepo GetFeatureDescIdList // {e.Message}");
                return null;
            }
        }

        public async Task<List<Cat_FeDesc_Realational>> GetList(Expression<Func<Cat_FeDesc_Realational, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.Cat_FeDesc_Realationals.Where(filter).AsNoTracking().ToListAsync()
                    : await _panelApiDbcontext.Cat_FeDesc_Realationals.AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_FeDesc_RelRepo GetList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Cat_FeDesc_Realational, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Cat_FeDesc_Realationals.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_FeDesc_RelRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Cat_FeDesc_Realational entity)
        {
            try
            {
                _panelApiDbcontext.Cat_FeDesc_Realationals.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_FeDesc_RelRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
