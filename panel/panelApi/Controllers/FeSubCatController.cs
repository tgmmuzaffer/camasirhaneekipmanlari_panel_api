using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
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
        private readonly ISubCategoryRepo _subCategoryRepo;
        private readonly ICat_Fe_RelRepo _cat_Fe_RelRepo;
        private readonly ILogger<FeSubCatController> _logger;

        public FeSubCatController(IFe_SubCat_RelRepo pr_FeDesc_RelRepo, ISubCategoryRepo subCategoryRepo, ICat_Fe_RelRepo cat_Fe_RelRepo, ILogger<FeSubCatController> logger)
        {
            _fe_SubCat_RelRepo = pr_FeDesc_RelRepo;
            _logger = logger;
            _subCategoryRepo = subCategoryRepo;
            _cat_Fe_RelRepo = cat_Fe_RelRepo;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Fe_SubCat_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createupdateFeSubCat")]
        public async Task<IActionResult> CreateUpdateFeSubCat([FromBody] List<Fe_SubCat_Relational> feature)
        {
            int subcatId = feature.Select(a => a.SubCategoryId).FirstOrDefault();
            List<int> willAddfeatureIds = new();
            List<Cat_Fe_Relational> cat_Fe_Relationals = new();
            var result = await _fe_SubCat_RelRepo.UpdateCreate(feature);
            if (!result)
            {
                _logger.LogError($"CreateFeSubCat/Fail__{feature[0].SubCategoryId} Subcat id'li FeSubCat oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Fe_SubCat_Relational could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateFeSubCat/Success__{feature[0].SubCategoryId} Subcat id'li FeSubCat oluşturuldu.");

            //var catId = await _subCategoryRepo.GetCategoryId(a => a.Id == feature[0].SubCategoryId);
            //if (catId != 0)
            //{
            //    var subCatList = await _subCategoryRepo.GetList(a => a.CategoryId == catId);
            //    foreach (var item in subCatList)
            //    {
            //        var fetureIds = await _fe_SubCat_RelRepo.GetFeatureIds(item.Id);
            //        if (fetureIds.Count > 0)
            //        {
            //            var differences = fetureIds.Except(willAddfeatureIds).ToList();
            //            willAddfeatureIds.AddRange(differences);
            //        }
            //    }
            //}

            //foreach (var itemWFI in willAddfeatureIds)
            //{
            //    Cat_Fe_Relational cat_Fe_Relational = new()
            //    {
            //        CategoryId= catId,
            //        FeatureId = itemWFI
            //    };
            //    cat_Fe_Relationals.Add(cat_Fe_Relational);
            //}

            //var cat_fe_res = await _cat_Fe_RelRepo.UpdateCreate(cat_Fe_Relationals);
            //if (!cat_fe_res)
            //{
            //    _logger.LogError($"CreateFeSubCat/Fail__{feature[0].SubCategoryId} Subcat id'li FeSubCat oluşturulurken hata meydana geldi.");
            //    ModelState.AddModelError("", "Fe_SubCat_Relational could not created");
            //    return StatusCode(500, ModelState);
            //}

            //_logger.LogWarning($"CreateFeSubCat/Success__{feature[0].SubCategoryId} Subcat id'li FeSubCat oluşturuldu.");


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
            var result = await _fe_SubCat_RelRepo.GetListWithRelatedEntity();
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

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteFeSubCatBysubCatId/{Id}")]
        public async Task<IActionResult> DeleteFeSubCatBysubCatId(int Id)
        {
            var feature = await _fe_SubCat_RelRepo.GetListWithRelatedEntity(a => a.SubCategoryId == Id);
            if (feature == null)
            {
                _logger.LogError($"DeleteFeSubCatBysubCatId__{Id} AltKategori Id'li FeSubCat bulunamdı.");
                ModelState.AddModelError("", "Fe_SubCat_Relational not found");
                return StatusCode(404, ModelState);
            }

            var result = await _fe_SubCat_RelRepo.RemoveMultiple(feature);
            if (!result)
            {
                _logger.LogError($"DeleteFeSubCatBysubCatId/Fail__{Id} AltKategori id'li FeSubCat silinirken hata oluştu.");
                ModelState.AddModelError("", "Fe_SubCat_Relational could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteFeSubCatBysubCatId/Success__{Id} AltKategori id'li FeSubCat silindi.");
            return NoContent();
        }

    }
}
