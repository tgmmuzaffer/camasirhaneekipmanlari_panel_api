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
    [Route("api/fesubcat")]
    [ApiController]
    public class FeSubCatController : ControllerBase
    {
        private readonly IFe_SubCat_RelRepo _fe_SubCat_RelRepo;
        private readonly ILogger<FeSubCatController> _logger;

        public FeSubCatController(IFe_SubCat_RelRepo pr_FeDesc_RelRepo, ILogger<FeSubCatController> logger)
        {
            _fe_SubCat_RelRepo = pr_FeDesc_RelRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Fe_SubCat_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createupdateFeSubCat")]
        public async Task<IActionResult> CreateUpdateFeSubCat([FromBody] List<Fe_SubCat_Relational> feature)
        {
            var result = await _fe_SubCat_RelRepo.UpdateCreate(feature);
            if (!result)
            {
                _logger.LogError($"CreateFeSubCat/Fail__{feature[0].SubCategory} Subcat id'li FeSubCat oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Fe_SubCat_Relational could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateFeSubCat/Success__{feature[0].SubCategory} Subcat id'li FeSubCat oluşturuldu.");
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Fe_SubCat_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getFeSubCat/{Id}")]
        public async Task<IActionResult> GetFeSubCat(int Id)
        {
            var result = await _fe_SubCat_RelRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetPrFedesc/Fail__{Id} Id'li FeSubCat bulunamdı.");
                ModelState.AddModelError("", "Fe_SubCat_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Fe_SubCat_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllFeSubCats")]
        public async Task<IActionResult> GetAllFeSubCats()
        {
            var result = await _fe_SubCat_RelRepo.GetList();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllFeSubCats/Fail__FeSubCat bulunamdı.", "");
                ModelState.AddModelError("", "Fe_SubCat_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        //[Authorize]
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Route("updateFeSubCat")]
        //public async Task<IActionResult> UpdateFeSubCat([FromBody] Fe_SubCat_Relational feature)
        //{
        //    var result = await _fe_SubCat_RelRepo.Update(feature);
        //    if (!result)
        //    {
        //        _logger.LogError($"UpdateFeSubCat/Fail__{feature.Id} id'li FeSubCat güncellenirken hata meydana geldi.");
        //        ModelState.AddModelError("", "Fe_SubCat_Relational could not updated");
        //        return StatusCode(500, ModelState);
        //    }

        //    _logger.LogWarning($"UpdateFeSubCat/Success__{feature.Id} id'li FeSubCat güncellendi");
        //    return NoContent();
        //}

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteFeSubCat/{Id}")]
        public async Task<IActionResult> DeleteFeSubCat(int Id)
        {
            var feature = await _fe_SubCat_RelRepo.Get(a => a.Id == Id);
            if (feature == null)
            {
                _logger.LogError($"DeleteFeSubCat__{Id} Id'li FeSubCat bulunamdı.");
                ModelState.AddModelError("", "Fe_SubCat_Relational not found");
                return StatusCode(404, ModelState);
            }

            var result = await _fe_SubCat_RelRepo.Delete(feature);
            if (!result)
            {
                _logger.LogError($"DeleteFeSubCat/Fail__{feature.Id} id'li FeSubCat silinirken hata oluştu.");
                ModelState.AddModelError("", "Fe_SubCat_Relational could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteFeSubCat/Success__{feature.Id} id'li FeSubCat silindi.");
            return NoContent();
        }
    }
}
