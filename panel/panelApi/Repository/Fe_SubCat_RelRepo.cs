﻿using Microsoft.EntityFrameworkCore;
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
    public class Fe_SubCat_RelRepo : IFe_SubCat_RelRepo
    {
        private readonly ILogger<Fe_SubCat_RelRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public Fe_SubCat_RelRepo(PanelApiDbcontext panelApiDbcontext, ILogger<Fe_SubCat_RelRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Fe_SubCat_Relational> Create(Fe_SubCat_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Fe_SubCat_Relationals.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"Fe_SubCat_RelRepo Create // {e.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateCreate(List<Fe_SubCat_Relational> entity)
        {
            using var transaction = _panelApiDbcontext.Database.BeginTransaction();

            try
            {
                var fe_sub_rels = await _panelApiDbcontext.Fe_SubCat_Relationals.ToListAsync();
                var featureIds = entity.Select(a => a.FeatureId).ToList();
                var subCatId = entity.Select(a => a.SubCategoryId).FirstOrDefault();
                var willDelete = fe_sub_rels.Where(a => a.SubCategoryId == subCatId && !featureIds.Contains(a.FeatureId)).ToList();
                var rel_Ids = fe_sub_rels.Where(a => a.SubCategoryId == subCatId).Select(b=>b.FeatureId);
                

                //delete
                if (willDelete.Count > 0)
                {
                    _panelApiDbcontext.Fe_SubCat_Relationals.RemoveRange(willDelete);
                    await _panelApiDbcontext.SaveChangesAsync();
                }
                else if (willDelete.Count == 0) //create
                {
                    var rel_feIds = fe_sub_rels.Select(a => a.FeatureId).ToList();
                    var rel_subId = fe_sub_rels.Select(a => a.SubCategoryId).Where(b => b == subCatId).FirstOrDefault();
                    var willCreate = entity.Where(a => !rel_Ids.Contains(a.FeatureId)).ToList();

                    _panelApiDbcontext.Fe_SubCat_Relationals.AddRange(willCreate);
                    await _panelApiDbcontext.SaveChangesAsync();
                }


                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Fe_SubCat_RelRepo UpdateCreate // {e.Message}");
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> Delete(Fe_SubCat_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Fe_SubCat_Relationals.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Fe_SubCat_RelRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Fe_SubCat_Relational> Get(Expression<Func<Fe_SubCat_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.Fe_SubCat_Relationals
                    .Include(a => a.Feature)
                    .Where(filter).FirstOrDefaultAsync() :
                    await _panelApiDbcontext.Fe_SubCat_Relationals
                    .Include(a => a.Feature)
                    .FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Fe_SubCat_RelRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<ICollection<Fe_SubCat_Relational>> GetList(Expression<Func<Fe_SubCat_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.Fe_SubCat_Relationals
                    .Include(a => a.Feature)
                    .Where(filter).ToListAsync() :
                    await _panelApiDbcontext.Fe_SubCat_Relationals
                    .Include(a => a.Feature)
                    .ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Fe_SubCat_RelRepo GetList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Fe_SubCat_Relational, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Fe_SubCat_Relationals.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Fe_SubCat_RelRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> IsPairExist(int featureId, int subCategoryId)
        {
            try
            {
                var featureRes = await _panelApiDbcontext.Fe_SubCat_Relationals.Where(f => f.FeatureId == featureId).Select(i => i.Id).ToListAsync();
                if (featureRes == null)
                    return false;

                var subCategoryRes = await _panelApiDbcontext.Fe_SubCat_Relationals.Where(f => f.SubCategoryId == subCategoryId).Select(i => i.Id).ToListAsync();
                if (subCategoryRes == null)
                    return false;

                var isMatch = featureRes.Where(a => subCategoryRes.Any(b => b == a));

                if (isMatch != null)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Fe_SubCat_RelRepo IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Fe_SubCat_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Fe_SubCat_Relationals.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Fe_SubCat_RelRepo Update // {e.Message}");
                return false;
            }
        }
    }
}