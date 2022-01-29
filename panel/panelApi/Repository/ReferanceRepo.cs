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
    public class ReferanceRepo : IReferanceRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ReferanceRepo(PanelApiDbcontext panelApiDbcontext)
        {
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
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

                throw new Exception(e.Message);
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
                throw new Exception(e.Message);
            }
        }
    }
}
