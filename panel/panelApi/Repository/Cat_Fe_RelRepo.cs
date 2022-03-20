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
    public class Cat_Fe_RelRepo : ICat_Fe_RelRepo
    {
        private readonly ILogger<Cat_Fe_RelRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public Cat_Fe_RelRepo(PanelApiDbcontext panelApiDbcontext, ILogger<Cat_Fe_RelRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }
        public async Task<Cat_Fe_Relational> Create(Cat_Fe_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Cat_Fe_Relatianals.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_Relatianals Create // {e.Message}");
                return null;
            }
        }



        public async Task<bool> Delete(Cat_Fe_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Cat_Fe_Relatianals.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_Relatianals Delete // {e.Message}");
                return false;
            }
        }

        public async Task<Cat_Fe_Relational> Get(Expression<Func<Cat_Fe_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Cat_Fe_Relatianals.Where(filter).FirstOrDefaultAsync() : await _panelApiDbcontext.Cat_Fe_Relatianals.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_Relatianals Get // {e.Message}");
                return null;
            }
        }



        public async Task<List<Cat_Fe_Relational>> GetList(Expression<Func<Cat_Fe_Relational, bool>> filter = null)
        {
            try
            {
                var result = filter != null
                    ? await _panelApiDbcontext.Cat_Fe_Relatianals.Where(filter).AsNoTracking().ToListAsync()
                    : await _panelApiDbcontext.Cat_Fe_Relatianals.AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_Relatianals GetList // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsExist(Expression<Func<Cat_Fe_Relational, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Cat_Fe_Relatianals.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_Relatianals IsExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> IsPairExist(int featureId, int categoryId)
        {
            try
            {
                var featureRes = await _panelApiDbcontext.Cat_Fe_Relatianals.Where(f => f.FeatureId == featureId).Select(i => i.Id).ToListAsync();
                if (featureRes == null)
                    return false;

                var categoryRes = await _panelApiDbcontext.Cat_Fe_Relatianals.Where(f => f.CategoryId == categoryId).Select(i => i.Id).ToListAsync();
                if (categoryRes == null)
                    return false;

                var isMatch = featureRes.Where(a => categoryRes.Any(b => b == a));

                if (isMatch != null)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_RelRepo IsPairExist // {e.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveMultiple(ICollection<Cat_Fe_Relational> entity)
        {
            try
            {
                _panelApiDbcontext.Cat_Fe_Relatianals.RemoveRange(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_Relatianals RemoveMultiple // {e.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Cat_Fe_Relational entity)
        {
            try
            {
                _panelApiDbcontext.Cat_Fe_Relatianals.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_Relatianals Update // {e.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateCreate(List<Cat_Fe_Relational> entity)
        {
            using var transaction = _panelApiDbcontext.Database.BeginTransaction();

            try
            {
                if (entity.Count > 0)
                {
                    var cat_fe_list = await _panelApiDbcontext.Cat_Fe_Relatianals.ToListAsync();
                    _panelApiDbcontext.RemoveRange(cat_fe_list);
                    await _panelApiDbcontext.SaveChangesAsync();

                     _panelApiDbcontext.AddRange(entity);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                //var fe_cat_rels = await _panelApiDbcontext.Cat_Fe_Relatianals.ToListAsync();
                //var featureIds = entity.Select(a => a.FeatureId).ToList();
                //var catId = entity.Select(a => a.CategoryId).FirstOrDefault();
                //var willDelete = fe_cat_rels.Where(a => a.CategoryId == catId && !featureIds.Contains(a.FeatureId)).ToList();
                //var rel_Ids = fe_cat_rels.Where(a => a.CategoryId == catId).Select(b => b.FeatureId);


                ////delete
                //if (willDelete.Count > 0)
                //{
                //    _panelApiDbcontext.Cat_Fe_Relatianals.RemoveRange(willDelete);
                //    await _panelApiDbcontext.SaveChangesAsync();
                //}
                //else if (willDelete.Count == 0) //create
                //{
                //    var rel_feIds = fe_cat_rels.Select(a => a.FeatureId).ToList();
                //    var rel_subId = fe_cat_rels.Select(a => a.CategoryId).Where(b => b == catId).FirstOrDefault();
                //    var willCreate = entity.Where(a => !rel_Ids.Contains(a.FeatureId)).ToList();

                //    _panelApiDbcontext.Cat_Fe_Relatianals.AddRange(willCreate);
                //    await _panelApiDbcontext.SaveChangesAsync();
                //}


                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cat_Fe_RelRepo UpdateCreate // {e.Message}");
                transaction.Rollback();
                return false;
            }
        }
    }
}
