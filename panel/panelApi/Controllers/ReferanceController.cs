using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.Repository.IRepository;
using System;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/referance")]
    [ApiController]
    public class ReferanceController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IReferanceRepo _referanceRepo;
        private readonly ILogger<ReferanceController> _logger;

        public ReferanceController(IReferanceRepo referanceRepo, IHostingEnvironment hostingEnvironment, ILogger<ReferanceController> logger)
        {
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
                _logger.LogError("CreateReferance", "Referans zaten mevcut");
                ModelState.AddModelError("", "Referance already exist");
                return StatusCode(404, ModelState);
            }

            string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + referanceDto.Name;
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(referanceDto.ImageData));
            Referance referance = new()
            {
                Description = referanceDto.Description,
                Id = referanceDto.Id,
                ImageName = referanceDto.ImageName,
                Name = referanceDto.Name,
                ShortDescription = referanceDto.ShortDescription
            };
            var result = await _referanceRepo.Create(referance);
            if (result == null)
            {
                _logger.LogError("CreateReferance_Fail", $"{referanceDto.Name} isimli Referans oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Referance could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("CreateReferance_Success", $"{referanceDto.Name} isimli Referans oluşturuldu.");
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
                _logger.LogError("GetReferance_Fail", $"{Id} Id'li Referans bulunamdı.");
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
            var result = await _referanceRepo.GetList();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllReferances_Fail", "Referanslar bulunamdı.");
                ModelState.AddModelError("", "Referance not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
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
                _logger.LogError("UpdateReferance", $"{referanceDto.Name} isimli_{referanceDto.Id} Id'li Referans bulunamdı.");
                ModelState.AddModelError("", "Referance not found");
                return StatusCode(404, ModelState);
            }

            Referance referance = new()
            {
                Description = referanceDto.Description,
                Id = referanceDto.Id,
                ImageName = referanceDto.ImageName,
                Name = referanceDto.Name,
                ShortDescription = referanceDto.ShortDescription
            };
            if (referance.ImageName != orjreferance.ImageName)
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjreferance.ImageName;
                System.IO.File.Delete(imgpath);
            }

            var result = await _referanceRepo.Update(referance);
            if (!result)
            {
                _logger.LogError("UpdateReferance_Fail", $"{referanceDto.Name} isimli Referans güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Referance could not updated");
                return StatusCode(500, ModelState);
            }

            string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + referanceDto.ImageName;
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(referanceDto.ImageData));           

            _logger.LogWarning("UpdateReferance_Success", $"{referanceDto.Name} isimli_{referanceDto.Id} id'li Referans güncellendi");
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
                _logger.LogError("DeleteReferance", $"{Id} Id'li Referans bulunamdı.");
                ModelState.AddModelError("", "Referance not found");
                return StatusCode(404, ModelState);
            }

            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + referance.ImageName;
            System.IO.File.Delete(imgpath);
            var result = await _referanceRepo.Delete(referance);
            if (!result)
            {
                _logger.LogError("DeleteReferance_Fail", $"{referance.Name} isimli Referans silinirken hata oluştu.");
                ModelState.AddModelError("", "Referance could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("DeleteReferance_Success", $"{referance.Name} isimli Referans silindi.");
            return NoContent();
        }
    }
}
