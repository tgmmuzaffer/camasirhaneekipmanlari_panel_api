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
    public class ContactRepo : IContactRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ContactRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Contact> Create(Contact entity)
        {
            _panelApiDbcontext.Contacts.Add(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(Contact entity)
        {
            _panelApiDbcontext.Contacts.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<Contact> Get(Expression<Func<Contact, bool>> filter = null)
        {
            var result = filter != null ? await _panelApiDbcontext.Contacts.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Contacts.FirstOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<Contact>> GetList(Expression<Func<Contact, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Contacts.Where(filter).ToListAsync() : await _panelApiDbcontext.Contacts.ToListAsync();
                return result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<bool> IsExist(Expression<Func<Contact, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Contacts.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<bool> Update(Contact entity)
        {
            _panelApiDbcontext.Contacts.Update(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }
    }
}
