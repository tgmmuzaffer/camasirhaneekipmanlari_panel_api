﻿using Microsoft.EntityFrameworkCore;
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
    public class CategoryRepo : ICategoryRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public CategoryRepo(PanelApiDbcontext panelApiDbcontext)
        {
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Category> Create(Category entity)
        {
            try
            {
                _panelApiDbcontext.Categories.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> Delete(Category entity)
        {
            try
            {
                _panelApiDbcontext.Categories.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Category> Get(Expression<Func<Category, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Categories.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Categories.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ICollection<Category>> GetList(Expression<Func<Category, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Categories.Where(filter).ToListAsync() : await _panelApiDbcontext.Categories.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> IsExist(Expression<Func<Category, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Categories.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> Update(Category entity)
        {
            try
            {
                _panelApiDbcontext.Categories.Update(entity);
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
