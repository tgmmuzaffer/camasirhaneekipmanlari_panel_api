using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using panelApi.Helpers;
using panelApi.Models;
using panelApi.RepoExtension;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IProductRepo _productRepo;
        private readonly IFeatureRepo _featureRepo;
        private readonly IFeatureDescriptionRepo _featureDescriptionRepo;
        private readonly IPr_Fe_RelRepo _pr_Fe_RelRepo;
        private readonly IPr_FeDesc_RelRepo _pr_FeDesc_RelRepo;
        private readonly ICat_Fe_RelRepo _cat_Fe_RelRepo;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMemoryCache memoryCache,
            IProductRepo productRepo,
            IFeatureRepo featureRepo,
            IPr_Fe_RelRepo pr_Fe_RelRepo,
            IPr_FeDesc_RelRepo pr_FeDesc_RelRepo,
            IFeatureDescriptionRepo featureDescriptionRepo,
            ICat_Fe_RelRepo cat_Fe_RelRepo,
            IWebHostEnvironment hostingEnvironment,
            ILogger<ProductController> logger)
        {
            _memoryCache = memoryCache;
            _productRepo = productRepo;
            _featureRepo = featureRepo;
            _featureDescriptionRepo = featureDescriptionRepo;
            _pr_Fe_RelRepo = pr_Fe_RelRepo;
            _pr_FeDesc_RelRepo = pr_FeDesc_RelRepo;
            _cat_Fe_RelRepo = cat_Fe_RelRepo;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var isexist = await _productRepo.IsExist(a => a.Name == product.Name);
            if (isexist)
            {
                _logger.LogError("CreateProduct__Ürün zaten mevcut");
                ModelState.AddModelError("", "Product already exist");
                return StatusCode(404, ModelState);
            }

            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + product.ImageName + ".webp";
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(product.ImagePath));
            }

            product.ImagePath = product.ImageName + ".webp";
            var result = await _productRepo.Create(product);
            if (result == null)
            {
                _logger.LogError($"CreateProduct/Fail__{product.Name} isimli Ürün oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Product could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateProduct/Success__{product.Name} isimli Ürün oluşturuldu.");
            return Ok(product.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getProduct/{Id}")]
        public async Task<IActionResult> GetProduct(int Id)
        {
            var result = await _productRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetProduct/Fail__{Id} Id'li Ürün bulunamdı.");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            var feature_Rel = await _pr_Fe_RelRepo.GetFetureIdList(a => a.ProductId == result.Id);
            result.Feature = await _featureRepo.GetListWithRelatedEntity(a => feature_Rel.Contains(a.Id));
            var featureDesc = await _pr_FeDesc_RelRepo.GetFeatureDescIdList(a => a.ProductId == result.Id);
            result.FeatureDescriptions = await _featureDescriptionRepo.GetListWithRelatedEntity(a => featureDesc.Contains(a.Id));
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ProductList))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getProductsByCatId/{Id}")]
        public async Task<IActionResult> GetProductsByCatId(int Id)
        {
            var cacheKey = Security.ProductByCatCache + Id;
            if (!_memoryCache.TryGetValue(cacheKey, out List<Product> product))
            {
                product = new();
                product = await _productRepo.GetListWithRelatedEntity(a => a.CategoryId == Id);
                foreach (var item in product)
                {
                    item.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetListWithRelatedEntity(a => a.ProductId == item.Id);
                    var feature_IdList = await _pr_Fe_RelRepo.GetFetureIdList(a => a.ProductId == item.Id);
                    item.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity(a => a.ProductId == item.Id);
                    var featureDesc_IdList = await _pr_FeDesc_RelRepo.GetFeatureDescIdList(a => a.ProductId == item.Id);

                    item.Feature = await _featureRepo.GetListWithRelatedEntity(s => feature_IdList.Contains(s.Id));
                    item.FeatureDescriptions = await _featureDescriptionRepo.GetListWithRelatedEntity(f => featureDesc_IdList.Contains(f.Id));
                }
                if (product.Count < 0)
                {
                    _logger.LogError("GetProductsByCatId/Fail__Ürünler bulunamdı.", "");
                    ModelState.AddModelError("", "Product not found");
                    return StatusCode(404, ModelState);
                }

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(cacheKey, product, cacheExpiryOptions);
            }
            return Ok(product);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ProductList))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getPagedProductsByCatId/")]
        public async Task<IActionResult> GetPagedProductsByCatId([FromQuery] ProductParameters productParameters)
        {
            string cache_key = "products";
            if (!_memoryCache.TryGetValue(cache_key, out List<Product> allproducts))
            {
                allproducts = new List<Product>();
                allproducts = await _productRepo.GetPagedList();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(cache_key, allproducts, cacheExpiryOptions);
            }

            var product = allproducts.FindAll(a => a.CategoryId == productParameters.Id);
            var result = PagedList<Product>.ToPagedList(product, productParameters.PageNumber, productParameters.PageSize);
            var fIdL = await _pr_Fe_RelRepo.GetListWithRelatedEntity();
            var fDIdL = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity();
            var features = await _featureRepo.GetListWithRelatedEntity();
            var featureDeswc = await _featureDescriptionRepo.GetListWithRelatedEntity();

            foreach (var item in result)
            {
                var feature_IdList = fIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureId).ToList();
                var featureDesc_IdList = fDIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureDescriptionId).ToList();

                item.Feature = features.Where(a => feature_IdList.Contains(a.Id)).ToList();
                item.FeatureDescriptions = featureDeswc.Where(a => featureDesc_IdList.Contains(a.Id)).ToList();
            }
            if (result.Count < 0)
            {
                _logger.LogError("GetProductsByCatId/Fail__Ürünler bulunamdı.", "");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ProductList))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getProductsBySubCatId/{Id}")]
        public async Task<IActionResult> GetProductsBySubCatId(int Id)
        {
            var cacheKey = Security.ProductBySubCatCache + Id;
            if (!_memoryCache.TryGetValue(cacheKey, out List<Product> product))
            {
                product = new();
                product = await _productRepo.GetListWithRelatedEntity(a => a.SubCategoryId == Id);
                foreach (var item in product)
                {
                    item.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetListWithRelatedEntity(a => a.ProductId == item.Id);
                    var feature_IdList = await _pr_Fe_RelRepo.GetFetureIdList(a => a.ProductId == item.Id);
                    item.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity(a => a.ProductId == item.Id);
                    var featureDesc_IdList = await _pr_FeDesc_RelRepo.GetFeatureDescIdList(a => a.ProductId == item.Id);

                    item.Feature = await _featureRepo.GetListWithRelatedEntity(s => feature_IdList.Contains(s.Id));
                    item.FeatureDescriptions = await _featureDescriptionRepo.GetListWithRelatedEntity(f => featureDesc_IdList.Contains(f.Id));
                }
                if (product.Count < 0)
                {
                    _logger.LogError("GetProductsBySubCatId/Fail__Ürünler bulunamdı.", "");
                    ModelState.AddModelError("", "Product not found");
                    return StatusCode(404, ModelState);
                }

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(cacheKey, product, cacheExpiryOptions);
            }
            return Ok(product);
        }
        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ProductList))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getPagedProductsBySubCatId")]
        public async Task<IActionResult> GetPagedProductsBySubCatId([FromQuery] ProductParameters productParameters)
        {
            string cache_key = "products";
            if (!_memoryCache.TryGetValue(cache_key, out List<Product> allproducts))
            {
                allproducts = new List<Product>();
                allproducts = await _productRepo.GetPagedList();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(cache_key, allproducts, cacheExpiryOptions);
            }
            var products = allproducts.FindAll(a => a.SubCategoryId == productParameters.Id);
            var result = PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);

            var fIdL = await _pr_Fe_RelRepo.GetListWithRelatedEntity();
            var fDIdL = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity();
            var features = await _featureRepo.GetListWithRelatedEntity();
            var featureDeswc = await _featureDescriptionRepo.GetListWithRelatedEntity();

            foreach (var item in result)
            {
                var feature_IdList = fIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureId).ToList();
                var featureDesc_IdList = fDIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureDescriptionId).ToList();

                item.Feature = features.Where(a => feature_IdList.Contains(a.Id)).ToList();
                item.FeatureDescriptions = featureDeswc.Where(a => featureDesc_IdList.Contains(a.Id)).ToList();
            }
            if (result.Count < 0)
            {
                _logger.LogError("GetProductsByCatId/Fail__Ürünler bulunamdı.", "");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ProductList))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getProductsByFeatureDescId/{Id}")]
        public async Task<IActionResult> GetProductsByFeatureDescId(int Id)
        {
            var cacheKey = Security.ProductByFeatureDesCache + Id;
            if (!_memoryCache.TryGetValue(cacheKey, out List<Product> product))
            {
                product = new();
                var productFeRels = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity(a => a.FeatureDescriptionId == Id);
                var productIds = productFeRels.Select(b => b.ProductId).ToList();
                product = await _productRepo.GetListWithRelatedEntity(a => productIds.Any(b => b == a.Id));
                foreach (var item in product)
                {
                    item.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetListWithRelatedEntity(a => a.ProductId == item.Id);
                    var feature_IdList = await _pr_Fe_RelRepo.GetFetureIdList(a => a.ProductId == item.Id);
                    item.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity(a => a.ProductId == item.Id);
                    var featureDesc_IdList = await _pr_FeDesc_RelRepo.GetFeatureDescIdList(a => a.ProductId == item.Id);

                    item.Feature = await _featureRepo.GetListWithRelatedEntity(s => feature_IdList.Contains(s.Id));
                    item.FeatureDescriptions = await _featureDescriptionRepo.GetListWithRelatedEntity(f => featureDesc_IdList.Contains(f.Id));
                }
                if (product.Count < 0)
                {
                    _logger.LogError("GetProductsBySubCatId/Fail__Ürünler bulunamdı.", "");
                    ModelState.AddModelError("", "Product not found");
                    return StatusCode(404, ModelState);
                }

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(cacheKey, product, cacheExpiryOptions);
            }
            return Ok(product);
        }
        
        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ProductList))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getPagedProductsByBrand")]
        public async Task<IActionResult> GetPagedProductsByBrand([FromQuery] ProductParameters productParameters)
        {
            string cache_key = "products";
            if (!_memoryCache.TryGetValue(cache_key, out List<Product> allproducts))
            {
                allproducts = new List<Product>();
                allproducts = await _productRepo.GetPagedList();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(cache_key, allproducts, cacheExpiryOptions);
            }

            var productFeRels = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity(a => a.FeatureDescriptionId == productParameters.Id);
            var productIds = productFeRels.Select(b => b.ProductId).ToList();
            var products = allproducts.FindAll(a => productIds.Any(b => b == a.Id));
            var result = PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);

            var fIdL = await _pr_Fe_RelRepo.GetListWithRelatedEntity();
            var fDIdL = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity();
            var features = await _featureRepo.GetListWithRelatedEntity();
            var featureDeswc = await _featureDescriptionRepo.GetListWithRelatedEntity();

            foreach (var item in result)
            {
                var feature_IdList = fIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureId).ToList();
                var featureDesc_IdList = fDIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureDescriptionId).ToList();

                item.Feature = features.Where(a => feature_IdList.Contains(a.Id)).ToList();
                item.FeatureDescriptions = featureDeswc.Where(a => featureDesc_IdList.Contains(a.Id)).ToList();
            }

            if (products.Count < 0)
            {
                _logger.LogError("getPagedProductsByBrnad/Fail__Ürünler bulunamdı.", "");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }
            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = new List<Product>();
            result = await _productRepo.GetListWithRelatedEntity();
            var fIdL = await _pr_Fe_RelRepo.GetListWithRelatedEntity();
            var fDIdL = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity();
            var features = await _featureRepo.GetListWithRelatedEntity();
            var featureDeswc = await _featureDescriptionRepo.GetListWithRelatedEntity();
            foreach (var item in result)
            {
                var feature_IdList = fIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureId).ToList();
                var featureDesc_IdList = fDIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureDescriptionId).ToList();
                item.Pr_Fe_Relationals = fIdL.FindAll(a => a.ProductId == item.Id);
                item.Pr_FeDesc_Relationals = fDIdL.FindAll(a => a.ProductId == item.Id);
                item.Feature = await _featureRepo.GetListWithRelatedEntity(s => feature_IdList.Contains(s.Id));
                item.FeatureDescriptions = await _featureDescriptionRepo.GetListWithRelatedEntity(f => featureDesc_IdList.Contains(f.Id));


                //item.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetList(a => a.ProductId == item.Id);
                //var feature_IdList = await _pr_Fe_RelRepo.GetFetureIdList(a => a.ProductId == item.Id);
                //item.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetList(a => a.ProductId == item.Id);
                //var featureDesc_IdList = await _pr_FeDesc_RelRepo.GetFeatureDescIdList(a => a.ProductId == item.Id);

                //item.Feature = await _featureRepo.GetList(s => feature_IdList.Contains(s.Id));
                //item.FeatureDescriptions = await _featureDescriptionRepo.GetList(f => featureDesc_IdList.Contains(f.Id));
            }
            if (result.Count < 0)
            {
                _logger.LogError("GetAllProducts/Fail__Ürünler bulunamdı.", "");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }
        
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("searchForProducts")]
        public async Task<IActionResult> SearchForProducts()
        {
            string cache_key = "searchForProducts";
            if (!_memoryCache.TryGetValue(cache_key, out List<Product> allproducts))
            {
                allproducts = new List<Product>();
                allproducts = await _productRepo.GetList();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMonths(3),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromDays(10)
                };
                _memoryCache.Set(cache_key, allproducts, cacheExpiryOptions);
            }

            //var fIdL = await _pr_Fe_RelRepo.GetListWithRelatedEntity();
            var fDIdL = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity();
            var featureDeswc = await _featureDescriptionRepo.GetListWithRelatedEntity();
            foreach (var item in allproducts)
            {
                //var feature_IdList = fIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureId).ToList();
                var featureDesc_IdList = fDIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureDescriptionId).ToList();
                //item.Pr_Fe_Relationals = fIdL.FindAll(a => a.ProductId == item.Id);
                //item.Pr_FeDesc_Relationals = fDIdL.FindAll(a => a.ProductId == item.Id);
                //item.Feature = await _featureRepo.GetListWithRelatedEntity(s => feature_IdList.Contains(s.Id));
                item.FeatureDescriptions = await _featureDescriptionRepo.GetListWithRelatedEntity(f => featureDesc_IdList.Contains(f.Id));
            }
            if (allproducts.Count < 0)
            {
                _logger.LogError("GetAllProducts/Fail__Ürünler bulunamdı.", "");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }
            return Ok(allproducts);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getPagedAllProducts/")]
        public async Task<IActionResult> GetPagedAllProducts([FromQuery] ProductParameters productParameters)
        {
            string cache_key = "products";
            if (!_memoryCache.TryGetValue(cache_key, out List<Product> allproducts))
            {
                allproducts = new List<Product>();
                allproducts = await _productRepo.GetPagedList();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(cache_key, allproducts, cacheExpiryOptions);
            }

            var result = PagedList<Product>.ToPagedList(allproducts, productParameters.PageNumber, productParameters.PageSize);
            //var fIdL = await _pr_Fe_RelRepo.GetList();
            var fDIdL = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity();
            //var features = await _featureRepo.GetList();
            var featureDeswc = await _featureDescriptionRepo.GetListWithRelatedEntity();
            foreach (var item in result)
            {
                //var feature_IdList = fIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureId).ToList();
                var featureDesc_IdList = fDIdL.Where(a => a.ProductId == item.Id).Select(b => b.FeatureDescriptionId).ToList();

                //item.Feature = features.Where(a => feature_IdList.Contains(a.Id)).ToList();
                item.FeatureDescriptions = featureDeswc.Where(a => featureDesc_IdList.Contains(a.Id)).ToList();
            }
            
            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            try
            {
                var orjprod = await _productRepo.Get(a => a.Id == product.Id);
            if (orjprod.Name == null)
            {
                _logger.LogError($"UpdateProduct__{product.Name} isimli_{product.Id} Id'li Ürün bulunamdı.");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            if (product.ImageName + ".webp" != orjprod.ImagePath)
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjprod.ImagePath;
                System.IO.File.Delete(imgpath);
            }

            if (product.Feature != null && product.FeatureDescriptions != null)
            {
                product.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity(a => a.ProductId == product.Id);
                product.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetListWithRelatedEntity(a => a.ProductId == product.Id);
            }

            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + product.ImageName + ".webp";
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(product.ImagePath));
            }

            product.ImagePath = product.ImageName + ".webp";
            var result = await _productRepo.Update(product);

            if (!result)
            {
                _logger.LogError($"UpdateProduct/Fail__{product.Name} isimli Ürün güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Product could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateProduct/Success__{product.Name} isimli_{product.Id} id'li Ürün güncellendi");
            return NoContent();
            }
            catch (Exception e)
            {
                return NoContent();

            }
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteProduct/{Id}")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var product = await _productRepo.Get(a => a.Id == Id);
            if (product == null)
            {
                _logger.LogError($"DeleteProduct__{Id} Id'li Ürün bulunamdı.");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + product.ImagePath;

            product.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetListWithRelatedEntity(a => a.ProductId == product.Id);
            product.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity(a => a.ProductId == product.Id);
            System.IO.File.Delete(imgpath);
            var result = await _productRepo.Delete(product);
            if (!result)
            {
                _logger.LogError($"DeleteProduct/Fail__{product.Name} isimli Ürün silinirken hata oluştu.");
                ModelState.AddModelError("", "Product could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteProduct/Success__{product.Name} isimli Ürün silindi.");
            return NoContent();
        }
    }
}
