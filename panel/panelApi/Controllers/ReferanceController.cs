using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/referance")]
    [ApiController]
    public class ReferanceController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IReferanceRepo _referanceRepo;
        private readonly ILogger<ReferanceController> _logger;
        private readonly IMemoryCache _memoryCache;

        public ReferanceController(IReferanceRepo referanceRepo, IWebHostEnvironment hostingEnvironment, ILogger<ReferanceController> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _referanceRepo = referanceRepo;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Referance))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createReferance")]
        public async Task<IActionResult> CreateReferance([FromBody] ReferanceDto referanceDto)
        {
            var isexist = await _referanceRepo.IsExist(a => a.Name == referanceDto.Name);
            if (isexist)
            {
                _logger.LogError("CreateReferance__Referans zaten mevcut");
                ModelState.AddModelError("", "Referance already exist");
                return StatusCode(404, ModelState);
            }

            if (!string.IsNullOrEmpty(referanceDto.ImageData))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + referanceDto.ImageName + ".webp";
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(referanceDto.ImageData));
            }

            Referance referance = new()
            {
                Description = referanceDto.Description,
                Id = referanceDto.Id,
                ImageName = referanceDto.ImageName + ".webp",
                Name = referanceDto.Name,
                ShortDescription = referanceDto.ShortDescription
            };
            var result = await _referanceRepo.Create(referance);
            if (result == null)
            {
                _logger.LogError($"CreateReferance/Fail__{referanceDto.Name} isimli Referans oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Referance could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateReferance/Success__{referanceDto.Name} isimli Referans oluşturuldu.");
            return Ok(referance.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Referance))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getReferance/{Id}")]
        public async Task<IActionResult> GetReferance(int Id)
        {
            var result = await _referanceRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetReferance/Fail__{Id} Id'li Referans bulunamdı.");
                ModelState.AddModelError("", "Referance not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Referance))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllReferances")]
        public async Task<IActionResult> GetAllReferances()
        {
            string key = "gar";
            var refrences = new List<Referance>();
            //var ur = HttpContext.Request.GetDisplayUrl();
            //if (ur.Contains("panel"))
            //{
            refrences = await _referanceRepo.GetListWithRelatedEntity();
                if (refrences.Count < 0)
                {
                    _logger.LogError("GetAllReferances/Fail__Referanslar bulunamdı.");
                    ModelState.AddModelError("", "Referance not found");
                    return StatusCode(404, ModelState);
                }
            //}
            //else if (_memoryCache.TryGetValue(key, out refrences))
            //{
            //    return Ok(refrences);

            //}
            //else
            //{
            //    refrences = await _referanceRepo.GetListWithRelatedEntity();
            //    if (refrences.Count < 0)
            //    {
            //        _logger.LogError("GetAllReferances/Fail__Referanslar bulunamdı.");
            //        ModelState.AddModelError("", "Referance not found");
            //        return StatusCode(404, ModelState);
            //    }
            //    var cacheExpiryOptions = new MemoryCacheEntryOptions
            //    {
            //        AbsoluteExpiration = DateTime.Now.AddHours(1),
            //        Priority = CacheItemPriority.High,
            //        SlidingExpiration = TimeSpan.FromMinutes(10)
            //    };
            //    _memoryCache.Set(key, refrences, cacheExpiryOptions);
            //}


            return Ok(refrences);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateReferance")]
        public async Task<IActionResult> UpdateReferance([FromBody] ReferanceDto referanceDto)
        {
            var orjreferance = await _referanceRepo.Get(a => a.Id == referanceDto.Id);
            if (orjreferance.Name == null)
            {
                _logger.LogError($"UpdateReferance__{referanceDto.Name} isimli_{referanceDto.Id} Id'li Referans bulunamdı.");
                ModelState.AddModelError("", "Referance not found");
                return StatusCode(404, ModelState);
            }

            Referance referance = new()
            {
                Description = referanceDto.Description,
                Id = referanceDto.Id,
                ImageName = referanceDto.ImageName + ".webp",
                Name = referanceDto.Name,
                ShortDescription = referanceDto.ShortDescription
            };
            if (referance.ImageName != orjreferance.ImageName)
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjreferance.ImageName + ".webp";
                System.IO.File.Delete(imgpath);
            }

            var result = await _referanceRepo.Update(referance);
            if (!result)
            {
                _logger.LogError($"UpdateReferance/Fail__{referanceDto.Name} isimli Referans güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Referance could not updated");
                return StatusCode(500, ModelState);
            }

            if (!string.IsNullOrEmpty(referanceDto.ImageData))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + referanceDto.ImageName;
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(referanceDto.ImageData));
            }


            _logger.LogWarning($"UpdateReferance/Success__{referanceDto.Name} isimli_{referanceDto.Id} id'li Referans güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteReferance/{Id}")]
        public async Task<IActionResult> DeleteReferance(int Id)
        {
            var referance = await _referanceRepo.Get(a => a.Id == Id);
            if (referance == null)
            {
                _logger.LogError($"DeleteReferance__{Id} Id'li Referans bulunamdı.");
                ModelState.AddModelError("", "Referance not found");
                return StatusCode(404, ModelState);
            }

            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + referance.ImageName;
            System.IO.File.Delete(imgpath);
            var result = await _referanceRepo.Delete(referance);
            if (!result)
            {
                _logger.LogError($"DeleteReferance/Fail__{referance.Name} isimli Referans silinirken hata oluştu.");
                ModelState.AddModelError("", "Referance could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteReferance/Success__{referance.Name} isimli Referans silindi.");
            return NoContent();
        }
    }
}
