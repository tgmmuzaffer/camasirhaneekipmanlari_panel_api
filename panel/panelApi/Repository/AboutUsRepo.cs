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
    public class AboutUsRepo : IAboutUsRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        private readonly ILogger<AboutUsRepo> _logger;

        public AboutUsRepo(PanelApiDbcontext panelApiDbcontext, ILogger<AboutUsRepo> logger)
        {
            _panelApiDbcontext = panelApiDbcontext;
            _logger = logger;
        }
        public async Task<AboutUs> Create(AboutUs entity)
        {
            try
            {
                _panelApiDbcontext.AboutUs.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"AboutUsRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(AboutUs entity)
        {
            try
            {
                _panelApiDbcontext.AboutUs.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"AboutUsRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<AboutUs> Get(Expression<Func<AboutUs, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.AboutUs.Where(filter).AsNoTracking().FirstOrDefaultAsync() :
                    await _panelApiDbcontext.AboutUs.AsNoTracking().FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"AboutUsRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<List<AboutUs>> GetList(Expression<Func<AboutUs, bool>> filter = null)
        {
            try
            {
                var result = filter != null 
                    ? await _panelApiDbcontext.AboutUs.Where(filter).AsNoTracking().ToListAsync()
                    : await _panelApiDbcontext.AboutUs.AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"AboutUsRepo GetList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<AboutUs, bool>> filter = null)
        {
            try
            {
                return await _panelApiDbcontext.AboutUs.AnyAsync(filter);
            }
            catch (Exception e)
            {
                _logger.LogError($"AboutUsRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(AboutUs entity)
        {
            try
            {
                _panelApiDbcontext.AboutUs.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"AboutUsRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
