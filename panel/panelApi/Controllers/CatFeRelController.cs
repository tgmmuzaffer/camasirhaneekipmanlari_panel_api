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
    [Route("api/fecat")]
    [ApiController]
    public class CatFeRelController : ControllerBase
    {
        private readonly ICat_Fe_RelRepo _cat_Fe_RelRepo;
        private readonly ILogger<CatFeRelController> _logger;

        public CatFeRelController(ICat_Fe_RelRepo cat_Fe_RelRepo, ILogger<CatFeRelController> logger)
        {
            _cat_Fe_RelRepo = cat_Fe_RelRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Cat_Fe_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createupdateFeCat")]
        public async Task<IActionResult> CreateUpdateFeCat([FromBody] List<Cat_Fe_Relational> feature)
        {
            var result = await _cat_Fe_RelRepo.UpdateCreate(feature);
            if (!result)
            {
                _logger.LogError($"CreateUpdateFeCat/Fail__{feature[0].Category} catagory id'li FeCat oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Cat_Fe_Relational could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateUpdateFeCat/Success__{feature[0].Category} catagory id'li FeCat oluşturuldu.");
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Cat_Fe_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getFeCat/{Id}")]
        public async Task<IActionResult> GetFeCat(int Id)
        {
            var result = await _cat_Fe_RelRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetFeCat/Fail__{Id} Id'li FeCat bulunamdı.");
                ModelState.AddModelError("", "Cat_Fe_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Cat_Fe_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllFeCats")]
        public async Task<IActionResult> GetAllFeCats()
        {
            var result = await _cat_Fe_RelRepo.GetListWithRelatedEntity();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllFeCats/Fail__FeCat bulunamdı.", "");
                ModelState.AddModelError("", "Cat_Fe_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Cat_Fe_Relational))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllFeCatsByCatId/{Id}")]
        public async Task<IActionResult> GetAllFeCatsByCatId(int Id)
        {
            var result = await _cat_Fe_RelRepo.GetListWithRelatedEntity(a => a.CategoryId == Id);
            if (result.Count < 0)
            {
                _logger.LogError("GetAllFeCatsByCatId/Fail__FeCat bulunamdı.", "");
                ModelState.AddModelError("", "Cat_Fe_Relational not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }


        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteFeCat/{Id}")]
        public async Task<IActionResult> DeleteFeCat(int Id)
        {
            var feature = await _cat_Fe_RelRepo.Get(a => a.Id == Id);
            if (feature == null)
            {
                _logger.LogError($"DeleteFeCat__{Id} Id'li FeCat bulunamdı.");
                ModelState.AddModelError("", "Cat_Fe_Relational not found");
                return StatusCode(404, ModelState);
            }

            var result = await _cat_Fe_RelRepo.Delete(feature);
            if (!result)
            {
                _logger.LogError($"DeleteFeCat/Fail__{feature.Id} id'li FeCat silinirken hata oluştu.");
                ModelState.AddModelError("", "Cat_Fe_Relational could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteFeCat/Success__{feature.Id} id'li FeCat silindi.");
            return NoContent();
        }


        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteFeCatByCatId/{Id}")]
        public async Task<IActionResult> DeleteFeCatByCatId(int Id)
        {
            var feature = await _cat_Fe_RelRepo.GetListWithRelatedEntity(a => a.CategoryId == Id);
            if (feature == null)
            {
                _logger.LogError($"DeleteFeCatByCatId__{Id } Kategori Id'li FeCat bulunamdı.");
                ModelState.AddModelError("", "Cat_Fe_Relational not found");
                return StatusCode(404, ModelState);
            }

            var result = await _cat_Fe_RelRepo.RemoveMultiple(feature);
            if (!result)
            {
                _logger.LogError($"DeleteFeCatByCatId/Fail__{Id} Kategori id'li FeCat silinirken hata oluştu.");
                ModelState.AddModelError("", "Cat_Fe_Relational could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteFeCatByCatId/Success__{Id} Kategori id'li FeCat silindi.");
            return NoContent();
        }
    }
}
