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
    public class PropertyTabRepo : IPropertyTabRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public PropertyTabRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext=panelApiDbcontext;
        }

        public async Task<PropertyTab> Create(PropertyTab entity)
        {
            _panelApiDbcontext.PropertyTabs.Add(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(PropertyTab entity)
        {
            _panelApiDbcontext.PropertyTabs.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<PropertyTab> Get(Expression<Func<PropertyTab, bool>> filter = null)
        {
            var result = filter == null ? await _panelApiDbcontext.PropertyTabs.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.PropertyTabs.FirstOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<PropertyTab>> GetList(Expression<Func<PropertyTab, bool>> filter = null)
        {
            var result = filter == null ? await _panelApiDbcontext.PropertyTabs.Where(filter).ToListAsync() : await _panelApiDbcontext.PropertyTabs.ToListAsync();
            return result;
        }

        public async Task<bool> IsExist(Expression<Func<PropertyTab, bool>> filter = null)
        {
            var result=await _panelApiDbcontext.PropertyTabs.AnyAsync(filter);
            return result;
        }

        public async Task<bool> Update(PropertyTab entity)
        {
            _panelApiDbcontext.Update(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }
    }
}
