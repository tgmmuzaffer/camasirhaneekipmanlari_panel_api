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
    public class PropertyDescRepo : IPropertyDesRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public PropertyDescRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext=panelApiDbcontext;
        }

        public async Task<PropertyDescription> Create(PropertyDescription entity)
        {
            _panelApiDbcontext.PropertyDescriptions.Add(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(PropertyDescription entity)
        {
            _panelApiDbcontext.PropertyDescriptions.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<PropertyDescription> Get(Expression<Func<PropertyDescription, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.PropertyDescriptions.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.PropertyDescriptions.FirstOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<PropertyDescription>> GetList(Expression<Func<PropertyDescription, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.PropertyDescriptions.Where(filter).ToListAsync() : await _panelApiDbcontext.PropertyDescriptions.ToListAsync();
            return result;
        }

        public async Task<bool> IsExist(Expression<Func<PropertyDescription, bool>> filter = null)
        {
            var result=await _panelApiDbcontext.PropertyDescriptions.AnyAsync(filter);
            return result;
        }

        public async Task<bool> Update(PropertyDescription entity)
        {
            _panelApiDbcontext.PropertyDescriptions.Update(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }
    }
}
