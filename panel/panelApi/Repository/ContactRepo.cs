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
    public class ContactRepo : IContactRepo
    {
        private readonly ILogger<ContactRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ContactRepo(PanelApiDbcontext panelApiDbcontext, ILogger<ContactRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Contact> Create(Contact entity)
        {
            try
            {
                _panelApiDbcontext.Contacts.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"ContactRepo Create // {e.Message}");
                return null;
            }

        }

        public async Task<bool> Delete(Contact entity)
        {
            try
            {
                _panelApiDbcontext.Contacts.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ContactRepo Delete // {e.Message}");
                return false;
            }

        }

        public async Task<Contact> Get(Expression<Func<Contact, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Contacts.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Contacts.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ContactRepo Get // {e.Message}");
                return null;
            }

        }

        public async Task<List<Contact>> GetList(Expression<Func<Contact, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Contacts.Where(filter).ToListAsync() : await _panelApiDbcontext.Contacts.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ContactRepo GetList // {e.Message}");
                return null;
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
                _logger.LogError($"ContactRepo IsExist // {e.Message}");
                return false;
            }

        }

        public async Task<bool> Update(Contact entity)
        {
            try
            {
                _panelApiDbcontext.Contacts.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ContactRepo Update // {e.Message}");
                return false;
            }

        }
    }
}
