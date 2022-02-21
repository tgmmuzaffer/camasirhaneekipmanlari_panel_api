using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/featureDescription")]
    [ApiController]
    public class FeatureDescriptionController : ControllerBase
    {
        private readonly IFeatureDescriptionRepo _featureDescriptionRepo;
        private readonly ILogger<FeatureDescriptionController> _logger;

        public FeatureDescriptionController(IFeatureDescriptionRepo featureDescriptionRepo, ILogger<FeatureDescriptionController> logger)
        {
            _featureDescriptionRepo = featureDescriptionRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(FeatureDescription))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createfeatureDescription")]
        public async Task<IActionResult> CreateFeatureDescription([FromBody] FeatureDescription featureDescription)
        {
            var isexist = await _featureDescriptionRepo.IsExist(a => a.FeatureDesc == featureDescription.FeatureDesc);
            if (isexist)
            {
                _logger.LogError("CreateFeatureDescription__ÖzellikAçıklaması zaten mevcut");
                ModelState.AddModelError("", "FeatureDescription already exist");
                return StatusCode(404, ModelState);
            }

            var result = await _featureDescriptionRepo.Create(featureDescription);
            if (result == null)
            {
                _logger.LogError($"CreateFeatureDescription/Fail__{featureDescription.FeatureDesc} isimli ÖzellikAçıklaması oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "FeatureDescription could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateFeatureDescription/Success__{featureDescription.FeatureDesc} isimli ÖzellikAçıklaması oluşturuldu.");
            return Ok(featureDescription.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(FeatureDescription))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getfeatureDescription/{Id}")]
        public async Task<IActionResult> GetFeatureDescription(int Id)
        {
            var result = await _featureDescriptionRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetFeatureDescription/Fail__{Id} Id'li ÖzellikAçıklaması bulunamdı.");
                ModelState.AddModelError("", "FeatureDescription not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(FeatureDescription))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllfeatureDescriptions")]
        public async Task<IActionResult> GetAllFeatureDescriptions()
        {
            var result = await _featureDescriptionRepo.GetList();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllFeatureDescriptions/Fail__ÖzellikAçıklamaları bulunamdı.", "");
                ModelState.AddModelError("", "FeatureDescription not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updatefeatureDescription")]
        public async Task<IActionResult> UpdateFeatureDescription([FromBody] FeatureDescription featureDescription)
        {
            var orjprod = await _featureDescriptionRepo.Get(a => a.Id == featureDescription.Id);
            if (orjprod.FeatureDesc == null)
            {
                _logger.LogError($"UpdateFeatureDescription__{featureDescription.FeatureDesc} isimli_{featureDescription.Id} Id'li ÖzellikAçıklaması bulunamdı.");
                ModelState.AddModelError("", "FeatureDescription not found");
                return StatusCode(404, ModelState);
            }

            var result = await _featureDescriptionRepo.Update(featureDescription);
            if (!result)
            {
                _logger.LogError($"UpdateFeatureDescription/Fail__{featureDescription.FeatureDesc} isimli ÖzellikAçıklaması güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "FeatureDescription could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateFeatureDescription/Success__{featureDescription.FeatureDesc} isimli_{featureDescription.Id} id'li ÖzellikAçıklaması güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deletefeatureDescription/{Id}")]
        public async Task<IActionResult> DeleteFeatureDescription(int Id)
        {
            var product = await _featureDescriptionRepo.Get(a => a.Id == Id);
            if (product == null)
            {
                _logger.LogError($"DeleteFeatureDescription__{Id} Id'li ÖzellikAçıklaması bulunamdı.");
                ModelState.AddModelError("", "FeatureDescription not found");
                return StatusCode(404, ModelState);
            }

            var result = await _featureDescriptionRepo.Delete(product);
            if (!result)
            {
                _logger.LogError($"DeleteFeatureDescription/Fail__{product.FeatureDesc} isimli ÖzellikAçıklaması silinirken hata oluştu.");
                ModelState.AddModelError("", "FeatureDescription could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteFeatureDescription/Success__{product.FeatureDesc} isimli ÖzellikAçıklaması silindi.");
            return NoContent();
        }
    }
}
