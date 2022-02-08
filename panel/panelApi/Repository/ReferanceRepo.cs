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
    public class ReferanceRepo : IReferanceRepo
    {
        private readonly ILogger<ReferanceRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ReferanceRepo(PanelApiDbcontext panelApiDbcontext, ILogger<ReferanceRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Referance> Create(Referance entity)
        {
            try
            {
                _panelApiDbcontext.Referances.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"ReferanceRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(Referance entity)
        {
            try
            {
                _panelApiDbcontext.Referances.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ReferanceRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Referance> Get(Expression<Func<Referance, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Referances.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Referances.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ReferanceRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<ICollection<Referance>> GetList(Expression<Func<Referance, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Referances.Where(filter).ToListAsync() : await _panelApiDbcontext.Referances.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ReferanceRepo GetList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Referance, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Referances.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ReferanceRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Referance entity)
        {
            try
            {
                _panelApiDbcontext.Referances.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ReferanceRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
