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
    public class SliderRepo : ISliderRepo
    {
        private readonly ILogger<SliderRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public SliderRepo(PanelApiDbcontext panelApiDbcontext, ILogger<SliderRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Slider> Create(Slider entity)
        {
            try
            {
                _panelApiDbcontext.Sliders.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"SliderRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(Slider entity)
        {
            try
            {
                _panelApiDbcontext.Sliders.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"SliderRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Slider> Get(Expression<Func<Slider, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Sliders.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Sliders.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"SliderRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<ICollection<Slider>> GetList(Expression<Func<Slider, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Sliders.Where(filter).ToListAsync() : await _panelApiDbcontext.Sliders.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"SliderRepo GetList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Slider, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Sliders.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"SliderRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Slider entity)
        {
            try
            {
                _panelApiDbcontext.Sliders.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"SliderRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
