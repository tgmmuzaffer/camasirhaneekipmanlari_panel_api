using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository;
using System;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/industry")]
    [ApiController]
    public class IndustryController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IIndustryRepo _industryRepo;
        private readonly ILogger<BlogController> _logger;
        public IndustryController(IWebHostEnvironment hostingEnvironment, IIndustryRepo industryRepo, ILogger<BlogController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _industryRepo = industryRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Industry))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createIndustry")]
        public async Task<IActionResult> CreateIndustry([FromBody] Industry industry)
        {
            if (!string.IsNullOrEmpty(industry.ImagePath))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + industry.ImagePath + ".webp";
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(industry.ImageData));
            }

            industry.ImagePath = industry.ImagePath + ".webp";
            var result = await _industryRepo.Create(industry);
            if (result == null)
            {
                _logger.LogError($"CreateIndustry/Fail__ oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Could not created");
                return StatusCode(500, ModelState);
            }


            _logger.LogWarning($"CreateIndustry/Success__  oluşturuldu");
            return Ok(industry.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Industry))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getIndustry/{Id}")]
        public async Task<IActionResult> GetIndustry(int Id)
        {
            var result = await _industryRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogWarning($"GetIndustry__ {Id} id'li Industry bulunamadı.");
                ModelState.AddModelError("", "Industry not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Industry))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getIndustries")]
        public async Task<IActionResult> GetIndustries()
        {
            var result = await _industryRepo.GetListWithRelatedEntity();
            if (result == null)
            {
                _logger.LogWarning($"GetIndustry__ Industry bulunamadı.");
                ModelState.AddModelError("", "Industries not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }


        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateIndustry")]
        public async Task<IActionResult> UpdateIndustry([FromBody] Industry industry)
        {
            try
            {
                var orjindustry = await _industryRepo.Get(a => a.Id == industry.Id);
                if (orjindustry == null)
                {
                    _logger.LogError($"UpdateIndustry {industry.Id} id'li Industry bulunamadı.");
                    ModelState.AddModelError("", "Industry not found");
                    return StatusCode(404, ModelState);
                }

                if (industry.ImagePath + ".webp" != orjindustry.ImagePath)
                {
                    var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjindustry.ImagePath;
                    System.IO.File.Delete(imgpath);
                }

                if (!string.IsNullOrEmpty(industry.ImagePath))
                {
                    string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + industry.ImagePath + ".webp"; ;
                    System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(industry.ImageData));
                }
                industry.ImagePath = industry.ImagePath + ".webp";
                var result = await _industryRepo.Update(industry);
                if (!result)
                {
                    _logger.LogError($"UpdateIndustry/Fail__ Industry güncellenirken hata meydana geldi.");
                    ModelState.AddModelError("", "Industry could not updated");
                    return StatusCode(500, ModelState);
                }
                _logger.LogWarning($"UpdateIndustry/Success__ {industry.Id} id'li industry güncellendi");
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
        [Route("deleteIndustry/{Id}")]
        public async Task<IActionResult> DeleteIndustry(int Id)
        {
            var industry = await _industryRepo.Get(a => a.Id == Id);
            if (industry == null)
            {
                _logger.LogError($"DeleteIndustry {industry.Id} id'li industry bulunamadı.");
                ModelState.AddModelError("", "Blog not found");
                return StatusCode(404, ModelState);
            }
            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + industry.ImagePath;
            System.IO.File.Delete(imgpath);
            var result = await _industryRepo.Delete(industry);
            if (!result)
            {
                _logger.LogError($"DeleteIndustry/Fail__ {Id} li industry silinirken hata oluştu.");
                ModelState.AddModelError("", "industry could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteIndustry/Succeess__{industry.Id} industry silindi.");
            return NoContent();
        }
    }
}
