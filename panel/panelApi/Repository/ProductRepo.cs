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
    public class ProductRepo : IProductRepo
    {
        private readonly ILogger<ProductRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public ProductRepo(PanelApiDbcontext panelApiDbcontext, ILogger<ProductRepo> logger)
        {
            _logger = logger;
            _panelApiDbcontext = panelApiDbcontext;
        }

        public async Task<Product> Create(Product entity)
        {
            using var transaction = _panelApiDbcontext.Database.BeginTransaction();
            try
            {
                _panelApiDbcontext.Products.Add(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                if (entity.Feature != null && entity.FeatureDescriptions != null)
                {
                    List<Pr_Fe_Relational> pr_FeList = new();
                    foreach (var _prFe in entity.Feature)
                    {
                        Pr_Fe_Relational pr_Fe = new() { ProductId = entity.Id, FeatureId = _prFe.Id };
                        pr_FeList.Add(pr_Fe);
                    }

                    _panelApiDbcontext.Pr_Fe_Relationals.AddRange(pr_FeList);
                    await _panelApiDbcontext.SaveChangesAsync();

                    List<Pr_FeDesc_Relational> pr_FeDescList = new();
                    foreach (var _prFeDesc in entity.FeatureDescriptions)
                    {
                        Pr_FeDesc_Relational pr_FeDesc = new() { ProductId = entity.Id, FeatureDescriptionId = _prFeDesc.Id };
                        pr_FeDescList.Add(pr_FeDesc);
                    }
                    _panelApiDbcontext.Pr_FeDesc_Relationals.AddRange(pr_FeDescList);
                    await _panelApiDbcontext.SaveChangesAsync();
                }


                transaction.Commit();
                return entity;

            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo Create // {e.Message}");
                transaction.Rollback();
                return null;
            }
        }

        public async Task<bool> Delete(Product entity)
        {
            using var transaction = _panelApiDbcontext.Database.BeginTransaction();
            try
            {
                _panelApiDbcontext.Products.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();

                if (entity.Pr_Fe_Relationals.Count > 0 && entity.Pr_FeDesc_Relationals.Count > 0)
                {
                    _panelApiDbcontext.Pr_Fe_Relationals.RemoveRange(entity.Pr_Fe_Relationals);
                    await _panelApiDbcontext.SaveChangesAsync();
                    _panelApiDbcontext.Pr_FeDesc_Relationals.RemoveRange(entity.Pr_FeDesc_Relationals);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo Delete // {e.Message}");
                transaction.Rollback();
                return false;
            }
        }

        public async Task<Product> Get(Expression<Func<Product, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.Products
                    .Include(a => a.Category).ThenInclude(b => b.SubCategories)
                    .Where(filter)
                    .OrderBy(a => a.Name)
                    .FirstOrDefaultAsync()
                    : await _panelApiDbcontext.Products
                    .Include(a => a.Category).ThenInclude(b => b.SubCategories)
                    .OrderBy(a => a.Name)
                    .FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<ICollection<Product>> GetList(Expression<Func<Product, bool>> filter = null)
        {
            try
            {
                var result = filter != null ?
                    await _panelApiDbcontext.Products
                    .Include(a => a.Category).ThenInclude(b => b.SubCategories)
                    .Where(filter)
                    .OrderBy(a => a.Name)
                    .ToListAsync()
                    : await _panelApiDbcontext.Products
                    .Include(a => a.Category).ThenInclude(b => b.SubCategories)
                    .OrderBy(a => a.Name)
                    .ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo GetList // {e.Message}");
                return null;
            }

        }

        public async Task<bool> IsExist(Expression<Func<Product, bool>> filter = null)
        {
            try
            {
                var result = await _panelApiDbcontext.Products.AnyAsync(filter);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo IsExist // {e.Message}");
                return false;
            }

        }

        public async Task<bool> Update(Product entity)
        {
            using var transaction = _panelApiDbcontext.Database.BeginTransaction();
            try
            {
                if(entity.Pr_Fe_Relationals != null && entity.Pr_FeDesc_Relationals != null)
                {
                    _panelApiDbcontext.Pr_Fe_Relationals.RemoveRange(entity.Pr_Fe_Relationals);
                    await _panelApiDbcontext.SaveChangesAsync();

                    _panelApiDbcontext.Pr_FeDesc_Relationals.RemoveRange(entity.Pr_FeDesc_Relationals);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                if (entity.Feature != null && entity.FeatureDescriptions != null)
                {
                    List<Pr_Fe_Relational> pr_FeList = new();
                    foreach (var _prFe in entity.Feature)
                    {
                        Pr_Fe_Relational pr_Fe = new() { ProductId = entity.Id, FeatureId = _prFe.Id };
                        pr_FeList.Add(pr_Fe);
                    }

                    _panelApiDbcontext.Pr_Fe_Relationals.AddRange(pr_FeList);
                    await _panelApiDbcontext.SaveChangesAsync();

                    List<Pr_FeDesc_Relational> pr_FeDescList = new();
                    foreach (var _prFeDesc in entity.FeatureDescriptions)
                    {
                        Pr_FeDesc_Relational pr_FeDesc = new() { ProductId = entity.Id, FeatureDescriptionId = _prFeDesc.Id };
                        pr_FeDescList.Add(pr_FeDesc);
                    }
                    _panelApiDbcontext.Pr_FeDesc_Relationals.AddRange(pr_FeDescList);
                    await _panelApiDbcontext.SaveChangesAsync();
                }

                _panelApiDbcontext.Products.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"ProductRepo Update // {e.Message}");
                return false;
            }

        }
    }
}
