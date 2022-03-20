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
    public class FeatureRepo : IFeatureRepo
    {
        private readonly ILogger<FeatureRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public FeatureRepo(PanelApiDbcontext panelApiDbcontext, ILogger<FeatureRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<Feature> Create(Feature entity)
        {
            try
            {
                _panelApiDbcontext.Features.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureRepo Create // {e.Message}");
                return null;
            }

        }

        public async Task<bool> Delete(Feature entity)
        {
            using var transaction = _panelApiDbcontext.Database.BeginTransaction();
            try
            {
                var catRels = await _panelApiDbcontext.Cat_Fe_Relatianals.AsNoTracking().Where(a => a.FeatureId == entity.Id).ToListAsync();
                if (catRels != null && catRels.Count > 0)
                {
                    _panelApiDbcontext.Cat_Fe_Relatianals.RemoveRange(catRels);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                var subcatRels = await _panelApiDbcontext.Fe_SubCat_Relationals.AsNoTracking().Where(a => a.FeatureId == entity.Id).ToListAsync();
                if (subcatRels != null && subcatRels.Count > 0)
                {
                    _panelApiDbcontext.Fe_SubCat_Relationals.RemoveRange(subcatRels);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                var fetureDescs = await _panelApiDbcontext.FeatureDescriptions.AsNoTracking().Where(a => a.FeatureId == entity.Id).ToListAsync();
                if(fetureDescs !=null && fetureDescs.Count > 0)
                {
                    var featureDescIds = fetureDescs.Select(a => a.Id).ToList();
                    foreach (var item in featureDescIds)
                    {
                        var prFeDescRels = await _panelApiDbcontext.Pr_FeDesc_Relationals.AsNoTracking().Where(a => a.FeatureDescriptionId == item).ToListAsync();
                        if(prFeDescRels != null && prFeDescRels.Count > 0)
                        {
                            _panelApiDbcontext.Pr_FeDesc_Relationals.RemoveRange(prFeDescRels);
                            await _panelApiDbcontext.SaveChangesAsync();
                        }
                    }

                    _panelApiDbcontext.FeatureDescriptions.RemoveRange(fetureDescs);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                var prFes = await _panelApiDbcontext.Pr_Fe_Relationals.AsNoTracking().Where(a => a.FeatureId == entity.Id).ToListAsync();
                if(prFes!=null && prFes.Count > 0)
                {
                    _panelApiDbcontext.Pr_Fe_Relationals.RemoveRange(prFes);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                _panelApiDbcontext.Features.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureRepo Delete // {e.Message}");
                transaction.Rollback();
                return false;
            }

        }

        public async Task<Feature> Get(Expression<Func<Feature, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.Features
                    .Include(a => a.SubCategories)
                    .Include(a => a.FeatureDescriptions)
                    .Where(filter)
                    .OrderBy(a => a.Name)
                    .FirstOrDefaultAsync() :
                    await _panelApiDbcontext.Features
                    .Include(a => a.SubCategories)
                    .Include(a => a.FeatureDescriptions)
                    .OrderBy(a => a.Name)
                    .FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureRepo Get // {e.Message}");
                return null;
            }

        }

        public async Task<List<Feature>> GetList(Expression<Func<Feature, bool>> filter = null)
        {
            try
            {
                var result = filter != null ? await _panelApiDbcontext.Features
                    .Include(a => a.SubCategories)
                    .Include(a => a.FeatureDescriptions)
                    .Where(filter)
                    .OrderBy(a => a.Name).ToListAsync()
                    : await _panelApiDbcontext.Features
                    .Include(a => a.SubCategories)
                    .Include(a => a.FeatureDescriptions)
                    .OrderBy(a => a.Name)
                    .ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureRepo GetList // {e.Message}");
                return null;
            }

        }

        public async Task<bool> IsExist(Expression<Func<Feature, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Features.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureRepo IsExist // {e.Message}");
                return false;
            }

        }

        public async Task<bool> Update(Feature entity)
        {
            try
            {
                _panelApiDbcontext.Features.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"FeatureRepo Update // {e.Message}");
                return false;
            }

        }
    }

}
