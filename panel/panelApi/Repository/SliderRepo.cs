using Microsoft.EntityFrameworkCore;
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
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public SliderRepo(PanelApiDbcontext panelApiDbcontext)
        {
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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

                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
            }
        }
    }
}
