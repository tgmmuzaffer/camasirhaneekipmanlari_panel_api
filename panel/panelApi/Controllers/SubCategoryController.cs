using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    
   [Route("api/subcategory")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryRepo _subCategoryRepo;
        private readonly IFe_SubCat_RelRepo  _fe_SubCat_RelRepo;
        private readonly ILogger<SubCategoryController> _logger;

        public SubCategoryController(ISubCategoryRepo productRepo,IFe_SubCat_RelRepo fe_SubCat_RelRepo, ILogger<SubCategoryController> logger)
        {
            _subCategoryRepo = productRepo;
            _fe_SubCat_RelRepo = fe_SubCat_RelRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(SubCategory))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createSubCategory")]
        public async Task<IActionResult> CreateSubCategory([FromBody] SubCategory subCategory)
        {
            var isexist = await _subCategoryRepo.IsExist(a => a.Name == subCategory.Name);
            if (isexist)
            {
                _logger.LogError("CreateSubCategory__AltKategori zaten mevcut");
                ModelState.AddModelError("", "SubCategory already exist");
                return StatusCode(404, ModelState);
            }
            
            var result = await _subCategoryRepo.Create(subCategory);
            if (result == null)
            {
                _logger.LogError($"CreateSubCategory/Fail__{subCategory.Name} isimli AltKategori oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "SubCategory could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateSubCategory/Success__{subCategory.Name} isimli AltKategori oluşturuldu.");
            return Ok(subCategory.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(SubCategory))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getSubCategory/{Id}")]
        public async Task<IActionResult> GetSubCategory(int Id)
        {
            var result = await _subCategoryRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetProduct/Fail__{Id} Id'li AltKategori bulunamdı.");
                ModelState.AddModelError("", "SubCategory not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(SubCategory))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getSubCategoryByCatId/{Id}")]
        public async Task<IActionResult> GetSubCategorybyCatId(int Id)
        {
            var result = await _subCategoryRepo.GetList(a => a.CategoryId == Id);
            if (result == null)
            {
                _logger.LogError($"GetSubCategorybyCatId/Fail__{Id} Id'li AltKategoriler bulunamdı.");
                ModelState.AddModelError("", "SubCategory not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }


        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(SubCategory))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getCatId/{Id}")]
        public async Task<IActionResult> GetCatId(int Id)
        {
            var result = await _subCategoryRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetCatId/Fail__{Id} Id'li AltKategoriler bulunamdı.");
                ModelState.AddModelError("", "SubCategory not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }






        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(SubCategory))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllSubCategories")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var result = await _subCategoryRepo.GetList();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllProducts/Fail__AltKategoriler bulunamdı.", "");
                ModelState.AddModelError("", "SubCategory not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateSubCategory")]
        public async Task<IActionResult> UpdateSubCategory([FromBody] SubCategory subCategory)
        {            
            var result = await _subCategoryRepo.Update(subCategory);           
            if (!result)
            {
                _logger.LogError($"UpdateSubCategory/Fail__{subCategory.Name} isimli AltKategori güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "SubCategory could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateSubCategory/Success__{subCategory.Name} isimli_{subCategory.Id} id'li AltKategori güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteSubCategory/{Id}")]
        public async Task<IActionResult> DeleteSubCategory(int Id)
        {
            var subcat = await _subCategoryRepo.Get(a => a.Id == Id);
            if (subcat == null)
            {
                _logger.LogError($"DeleteSubCategory__{Id} Id'li AltKategori bulunamdı.");
                ModelState.AddModelError("", "SubCategory not found");
                return StatusCode(404, ModelState);
            }

            //var subcatrel = await _fe_SubCat_RelRepo.GetList(a => a.SubCategoryId == Id);
            //var rel_delete_result = subcatrel.Count > 0 && await _fe_SubCat_RelRepo.RemoveMultiple(subcatrel);

            var delete_result = await _subCategoryRepo.Delete(subcat);
            if (!delete_result)
            {
                _logger.LogError($"DeleteSubCategory/Fail__{subcat.Name} isimli AltKategori silinirken hata oluştu.");
                ModelState.AddModelError("", "SubCategory could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteSubCategory/Success__{subcat.Name} isimli AltKategori silindi.");
            return NoContent();
        }
    }
}
