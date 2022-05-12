using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/aboutus")]
    [ApiController]
    public class AboutUsController : ControllerBase
    {
        private readonly IAboutUsRepo _aboutusrepo;
        private readonly ILogger<AboutUsController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMemoryCache _memoryCache;


        public AboutUsController(IAboutUsRepo aboutusrepo, IWebHostEnvironment hostingEnvironment, ILogger<AboutUsController> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _aboutusrepo = aboutusrepo;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;

        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(AboutUs))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createAboutUs")]
        public async Task<IActionResult> CreateAboutUs([FromBody] AboutUs aboutUs)
        {
            bool isOk = true;
            var isexist = await _aboutusrepo.IsExist(a => a.Title == aboutUs.Title);
            if (isexist)
            {
                _logger.LogError("CreateAboutUs__Kategori zaten mevcut", "");
                ModelState.AddModelError("", "AboutUs already exist");
                return StatusCode(404, ModelState);
            }

            #region ImageUpload
            if (!string.IsNullOrEmpty(aboutUs.ImagePath1))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImageName1;
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(aboutUs.ImagePath1));
                aboutUs.ImagePath1 = aboutUs.ImageName1;
            }
            if (!string.IsNullOrEmpty(aboutUs.ImagePath2))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImageName2;
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(aboutUs.ImagePath2));
                aboutUs.ImagePath2 = aboutUs.ImageName2;
            }
            if (!string.IsNullOrEmpty(aboutUs.ImagePath3))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImageName3;
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(aboutUs.ImagePath3));
                aboutUs.ImagePath3 = aboutUs.ImageName3;
            }
            #endregion

            var result = await _aboutusrepo.Create(aboutUs);

            if (result == null && isOk == true)
            {
                _logger.LogError($"CreateAboutUs/Fail__{aboutUs.Title} isimli Hakkımızda oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "AboutUs could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("CreateAboutUs_Success", $"{aboutUs.Title} isimli Hakkımızda oluşturuldu.");
            return Ok(aboutUs.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(AboutUs))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAboutUs")]
        public async Task<IActionResult> GetAboutUs()
        {
            //string key = "gaus";
            var aboutus = new AboutUs();
            //var ur = HttpContext.Request.GetDisplayUrl();
            //if (ur.Contains("panel"))
            //{
                aboutus = await _aboutusrepo.Get();
                if (aboutus == null)
                {
                    _logger.LogError($"GetAboutUs/Fail__ Hakkımızda bulunamdı.");
                    ModelState.AddModelError("", "AboutUs not found");
                    return StatusCode(404, ModelState);
                }
            //}
            //else if (_memoryCache.TryGetValue(key, out aboutus))
            //{
            //    return Ok(aboutus);

            //}
            //else
            //{
            //    aboutus = await _aboutusrepo.Get();
            //    if (aboutus == null)
            //    {
            //        _logger.LogError($"GetAboutUs/Fail__ Hakkımızda bulunamdı.");
            //        ModelState.AddModelError("", "AboutUs not found");
            //        return StatusCode(404, ModelState);
            //    }

            //    var cacheExpiryOptions = new MemoryCacheEntryOptions
            //    {
            //        AbsoluteExpiration = DateTime.Now.AddHours(1),
            //        Priority = CacheItemPriority.High,
            //        SlidingExpiration = TimeSpan.FromMinutes(10)
            //    };
            //    _memoryCache.Set(key, aboutus, cacheExpiryOptions);
            //}

            return Ok(aboutus);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateAboutUs")]
        public async Task<IActionResult> UpdateAboutUs([FromBody] AboutUs aboutUs)
        {
            var orjAboutUs = await _aboutusrepo.Get(a => a.Id == aboutUs.Id);
            if (orjAboutUs == null)
            {
                _logger.LogError($"UpdateAboutUs__{aboutUs.Title} isimli_{aboutUs.Id} Id'li Hakkımızda bulunamdı.");
                ModelState.AddModelError("", "AboutUs not found");
                return StatusCode(404, ModelState);
            }

            #region ImageUpload
            if ((orjAboutUs.ImagePath1 != null) && (aboutUs.ImageName1 != orjAboutUs.ImagePath1))
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjAboutUs.ImagePath1;
                System.IO.File.Delete(imgpath);
            }

            if (!string.IsNullOrEmpty(aboutUs.ImagePath1))
            {
                string filePath1 = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImageName1;
                System.IO.File.WriteAllBytes(filePath1, Convert.FromBase64String(aboutUs.ImagePath1));
                aboutUs.ImagePath1 = aboutUs.ImageName1;
            }


            if ((orjAboutUs.ImagePath2 != null) && (aboutUs.ImageName2 != orjAboutUs.ImagePath2))
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjAboutUs.ImagePath2;
                System.IO.File.Delete(imgpath);
            }

            if (!string.IsNullOrEmpty(aboutUs.ImagePath2))
            {
                string filePath2 = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImageName2;
                System.IO.File.WriteAllBytes(filePath2, Convert.FromBase64String(aboutUs.ImagePath2));
                aboutUs.ImagePath2 = aboutUs.ImageName2;
            }



            if ((orjAboutUs.ImagePath3 != null) && (aboutUs.ImageName3 != orjAboutUs.ImagePath3))
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjAboutUs.ImagePath3;
                System.IO.File.Delete(imgpath);
            }

            if (!string.IsNullOrEmpty(aboutUs.ImagePath3))
            {
                string filePath3 = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImageName3;
                System.IO.File.WriteAllBytes(filePath3, Convert.FromBase64String(aboutUs.ImagePath3));
                aboutUs.ImagePath3 = aboutUs.ImageName3;
            }

            #endregion


            var result = await _aboutusrepo.Update(aboutUs);
            if (!result)
            {
                _logger.LogError($"UpdateAboutUs/Fail__{aboutUs.Title} isimli Hakkımızda güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "AboutUs could not updated");
                return StatusCode(500, ModelState);
            }


            _logger.LogWarning($"UpdateAboutUs/Success__{aboutUs.Title} isimli_{aboutUs.Id} id'li Hakkımızda güncellendi.");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteAboutUs/{Id}")]
        public async Task<IActionResult> DeleteAboutUs(int Id)
        {
            var aboutUs = await _aboutusrepo.Get(a => a.Id == Id);
            if (aboutUs == null)
            {
                _logger.LogError($"DeleteAboutUs__{Id} Id'li Hakkımızda bulunamdı.");
                ModelState.AddModelError("", "AboutUs not found");
                return StatusCode(404, ModelState);
            }

            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImagePath1;
            System.IO.File.Delete(imgpath);
            var imgpath2 = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImagePath2;
            System.IO.File.Delete(imgpath2);
            var imgpath3 = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + aboutUs.ImagePath3;
            System.IO.File.Delete(imgpath3);

            var result = await _aboutusrepo.Delete(aboutUs);
            if (!result)
            {
                _logger.LogError($"DeleteAboutUs/Fail__{aboutUs.Title} başlıklı Hakkımızda silinirken hata oluştu.");
                ModelState.AddModelError("", "AboutUs could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteAboutUs/Success__{aboutUs.Title} başlıklı Hakkımızda silindi.");
            return NoContent();
        }
    }
}
