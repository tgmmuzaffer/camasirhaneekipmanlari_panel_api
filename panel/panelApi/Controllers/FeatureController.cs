using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/feature")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureRepo _featureRepo;
        private readonly ILogger<FeatureController> _logger;
        private readonly IMemoryCache _memoryCache;

        public FeatureController(IFeatureRepo productRepo, ILogger<FeatureController> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _featureRepo = productRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Feature))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createFeature")]
        public async Task<IActionResult> CreateFeature([FromBody] Feature feature)
        {
            var isexist = await _featureRepo.IsExist(a => a.Name == feature.Name);
            if (isexist)
            {
                _logger.LogError("CreateFeature__Özellik zaten mevcut");
                ModelState.AddModelError("", "Feature already exist");
                return StatusCode(404, ModelState);
            }

            var result = await _featureRepo.Create(feature);
            if (result == null)
            {
                _logger.LogError($"CreateFeature/Fail__{feature.Name} isimli Özellik oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Feature could not created");
                return StatusCode(500, ModelState);
            }

            var featureList = await _featureRepo.GetList();
            _logger.LogWarning($"CreateFeature/Success__{feature.Name} isimli Özellik oluşturuldu.");
            return Ok(featureList);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Feature))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getFeature/{Id}")]
        public async Task<IActionResult> GetFeature(int Id)
        {
            var result = await _featureRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetFeature/Fail__{Id} Id'li Özellik bulunamdı.");
                ModelState.AddModelError("", "Feature not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Feature))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getFeatureByName/{name}")]
        public async Task<IActionResult> GetFeatureByName(string name)
        {
            string key = "gaf";
            var feature = new Feature();
            var ur = HttpContext.Request.GetDisplayUrl();
            if (ur.Contains("panel"))
            {
                feature = await _featureRepo.Get(a => a.Name == name);
                if (feature == null)
                {
                    _logger.LogError($"GetFeature/Fail__{name} Id'li Özellik bulunamdı.");
                    ModelState.AddModelError("", "Feature not found");
                    return StatusCode(404, ModelState);
                }
            }
            else if (_memoryCache.TryGetValue(key, out feature))
            {
                return Ok(feature);

            }
            else
            {
                feature = await _featureRepo.Get(a => a.Name == name);
                if (feature == null)
                {
                    _logger.LogError($"GetFeature/Fail__{name} Id'li Özellik bulunamdı.");
                    ModelState.AddModelError("", "Feature not found");
                    return StatusCode(404, ModelState);
                }

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(key, feature, cacheExpiryOptions);
            }

            return Ok(feature);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Feature))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllFeatures")]
        public async Task<IActionResult> GetAllFeatures()
        {
            string key = "gaf";
            var features = new List<Feature>();
            var ur = HttpContext.Request.GetDisplayUrl();
            if (ur.Contains("panel"))
            {
                features = await _featureRepo.GetList();
                if (features == null)
                {
                    _logger.LogError("GetAllFeatures/Fail__Özellikler bulunamdı.", "");
                    ModelState.AddModelError("", "Feature not found");
                    return StatusCode(404, ModelState);
                }
            }
            else if (_memoryCache.TryGetValue(key, out features))
            {
                return Ok(features);

            }
            else
            {
                features = await _featureRepo.GetList();
                if (features == null)
                {
                    _logger.LogError("GetAllFeatures/Fail__Özellikler bulunamdı.", "");
                    ModelState.AddModelError("", "Feature not found");
                    return StatusCode(404, ModelState);
                }

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(key, features, cacheExpiryOptions);
            }


            return Ok(features);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Feature))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getFeaturesBySubCatId/{Id}")]
        public async Task<IActionResult> GetAllFeaturesBySubCatId(int Id)
        {
            var result = await _featureRepo.GetList(a => a.SubCategories.All(b => b.Id == Id));
            if (result.Count < 0)
            {
                _logger.LogError("GetAllFeatures/Fail__Özellikler bulunamdı.", "");
                ModelState.AddModelError("", "Feature not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateFeature")]
        public async Task<IActionResult> UpdateFeature([FromBody] Feature feature)
        {
            var result = await _featureRepo.Update(feature);
            if (!result)
            {
                _logger.LogError($"UpdateFeature/Fail__{feature.Name} isimli Özellik güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Feature could not updated");
                return StatusCode(500, ModelState);
            }

            var featureList = await _featureRepo.GetList();
            if (featureList == null)
            {
                _logger.LogError("UpdateFeature/Fail__Özellikler bulunamdı.", "");
                ModelState.AddModelError("", "Feature not found");
                return StatusCode(404, ModelState);
            }

            _logger.LogWarning($"UpdateFeature/Success__{feature.Name} isimli_{feature.Id} id'li Özellik güncellendi");
            return Ok(featureList);
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteFeature/{Id}")]
        public async Task<IActionResult> DeleteFeature(int Id)
        {
            var feature = await _featureRepo.Get(a => a.Id == Id);
            if (feature == null)
            {
                _logger.LogError($"DeleteFeature__{Id} Id'li Özellik bulunamdı.");
                ModelState.AddModelError("", "Feature not found");
                return StatusCode(404, ModelState);
            }

            var result = await _featureRepo.Delete(feature);
            if (!result)
            {
                _logger.LogError($"DeleteFeature/Fail__{feature.Name} isimli Özellik silinirken hata oluştu.");
                ModelState.AddModelError("", "Feature could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteFeature/Success__{feature.Name} isimli Özellik silindi.");
            return NoContent();
        }
    }
}
