using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/prfe")]
    [ApiController]
    public class PrFeRelController : ControllerBase
    {
        private readonly IPr_Fe_RelRepo _pr_Fe_RelRepo;
        private readonly ILogger<PrFeRelController> _logger;

        public PrFeRelController(IPr_Fe_RelRepo pr_Fe_RelRepo, ILogger<PrFeRelController> logger)
        {
            _pr_Fe_RelRepo = pr_Fe_RelRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Pr_Fe_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createMultiplePrFe")]
        public async Task<IActionResult> CreatePrFe([FromBody] List<Pr_Fe_Relational> feature)
        {
            var result = await _pr_Fe_RelRepo.CreateMultiple(feature);
            if (!result)
            {
                _logger.LogError($"CreatePrFe/Fail__{feature[0].ProductId} product id'li Pr_Fe oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Pr_Fe_Relational could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreatePrFe/Success__{feature[0].ProductId} product id'li Pr_Fe oluşturuldu.");
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Pr_Fe_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getPrFe/{Id}")]
        public async Task<IActionResult> GetPrFe(int Id)
        {
            var result = await _pr_Fe_RelRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetPrFe/Fail__{Id} Id'li Pr_Fe bulunamdı.");
                ModelState.AddModelError("", "Pr_Fe_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Pr_Fe_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllPrFes")]
        public async Task<IActionResult> GetAllPrFes()
        {
            var result = await _pr_Fe_RelRepo.GetListWithRelatedEntity();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllPrFes/Fail__Pr_Fe bulunamdı.", "");
                ModelState.AddModelError("", "Pr_Fe_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updatePrFe")]
        public async Task<IActionResult> UpdatePrFe([FromBody] Pr_Fe_Relational feature)
        {
            var result = await _pr_Fe_RelRepo.Update(feature);
            if (!result)
            {
                _logger.LogError($"UpdateFeFedesc/Fail__{feature.Id} id'li Pr_Fe güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Pr_Fe_Relational could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateFeFedesc/Success__{feature.Id} id'li Pr_Fe güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deletePrFe/{Id}")]
        public async Task<IActionResult> DeletePrFe(int Id)
        {
            var feature = await _pr_Fe_RelRepo.Get(a => a.Id == Id);
            if (feature == null)
            {
                _logger.LogError($"DeletePrFe {Id} Id'li Pr_Fe bulunamdı.");
                ModelState.AddModelError("", "Pr_Fe_Relational not found");
                return StatusCode(404, ModelState);
            }

            var result = await _pr_Fe_RelRepo.Delete(feature);
            if (!result)
            {
                _logger.LogError($"DeletePrFe/Fail__{feature.Id} id'li Pr_Fe silinirken hata oluştu.");
                ModelState.AddModelError("", "Pr_Fe_Relational could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeletePrFe/Success__{feature.Id} id'li Pr_Fe silindi.");
            return NoContent();
        }
    }
}
