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
    [Route("api/prfedesc")]
    [ApiController]
    public class PrFeDescController : ControllerBase
    {
        private readonly IPr_FeDesc_RelRepo _pr_FeDesc_RelRepo;
        private readonly ILogger<PrFeDescController> _logger;

        public PrFeDescController(IPr_FeDesc_RelRepo pr_FeDesc_RelRepo, ILogger<PrFeDescController> logger)
        {
            _pr_FeDesc_RelRepo = pr_FeDesc_RelRepo;
            _logger = logger;
        }
        /*
         * create update de liste tipi gelen data ya dikkat 
         * yeni eklenirken her data kotrole edilmeli varsa update 
         * yoksa create yapacak şekilde akış güncellenmeli
         */
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Pr_FeDesc_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createMultiplePrFedesc")]
        public async Task<IActionResult> CreateMultiplePrFedesc([FromBody] List<Pr_FeDesc_Relational> feature)
        {

            var result = await _pr_FeDesc_RelRepo.CreateMultiple(feature);
            if (!result)
            {
                _logger.LogError($"CreateMultiplePrFedesc/Fail__{feature[0].ProductId} product id'li Pr_FeDesc oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Pr_FeDesc_Relational could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateMultiplePrFedesc/Success__{feature[0].ProductId} product id'li Pr_FeDesc oluşturuldu.");
            return Ok(feature[0].ProductId);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Pr_FeDesc_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getPrFedesc/{Id}")]
        public async Task<IActionResult> GetPrFedesc(int Id)
        {
            var result = await _pr_FeDesc_RelRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetPrFedesc/Fail__{Id} Id'li Pr_FeDesc bulunamdı.");
                ModelState.AddModelError("", "Pr_FeDesc_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Pr_FeDesc_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllPrFedescs")]
        public async Task<IActionResult> GetAllPrFedescs()
        {
            var result = await _pr_FeDesc_RelRepo.GetListWithRelatedEntity();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllFeFedescs/Fail__Pr_FeDesc bulunamdı.", "");
                ModelState.AddModelError("", "Pr_FeDesc_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updatePrFedesc")]
        public async Task<IActionResult> UpdatePrFedesc([FromBody] Pr_FeDesc_Relational feature)
        {
            var result = await _pr_FeDesc_RelRepo.Update(feature);
            if (!result)
            {
                _logger.LogError($"UpdatePrFedesc/Fail__{feature.Id} id'li Pr_FeDesc güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Pr_FeDesc_Relational could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateFeFedesc/Success__{feature.Id} id'li Pr_FeDesc güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deletePrFedesc/{Id}")]
        public async Task<IActionResult> DeletePrFedesc(int Id)
        {
            var feature = await _pr_FeDesc_RelRepo.Get(a => a.Id == Id);
            if (feature == null)
            {
                _logger.LogError($"DeleteFeFedesc__{Id} Id'li Pr_FeDesc bulunamdı.");
                ModelState.AddModelError("", "Pr_FeDesc_Relational not found");
                return StatusCode(404, ModelState);
            }

            var result = await _pr_FeDesc_RelRepo.Delete(feature);
            if (!result)
            {
                _logger.LogError($"DeletePrFedesc/Fail__{feature.Id} id'li Pr_FeDesc silinirken hata oluştu.");
                ModelState.AddModelError("", "Pr_FeDesc_Relational could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeletePrFedesc/Success__{feature.Id} id'li Pr_FeDesc silindi.");
            return NoContent();
        }
    }
}
