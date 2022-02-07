using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepo _tagRepo;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagRepo tagRepo, ILogger<TagController> logger)
        {
            _tagRepo = tagRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Tag))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createTag")]
        public async Task<IActionResult> CreateTag([FromBody] Tag tag)
        {
            var isexist = await _tagRepo.IsExist(a => a.Name == tag.Name);
            if (isexist)
            {
                _logger.LogError("CreateTag", "tag zaten mevcut");
                ModelState.AddModelError("", "Tag already exist");
                return StatusCode(404, ModelState);
            }

            var result = await _tagRepo.Create(tag);
            if (result == null)
            {
                _logger.LogError("CreateTag_Fail", $"{tag.Name} isimli Tag oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Tag could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("CreateTag_Success", $"{tag.Name} isimli Tag oluşturuldu.");
            return Ok(tag.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Tag))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getTag/{Id}")]
        public async Task<IActionResult> GetTag(int Id)
        {
            var result = await _tagRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError("GetTag_Fail", $"{Id} Id'li Tag bulunamdı.");
                ModelState.AddModelError("", "Tag not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Tag))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllTags")]
        public async Task<IActionResult> GetAllTags()
        {
            var result = await _tagRepo.GetList();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllTags_Fail", "Taglar bulunamdı.");
                ModelState.AddModelError("", "Tag not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateTag")]
        public async Task<IActionResult> UpdateTag([FromBody] Tag tag)
        {
            var isexist = await _tagRepo.IsExist(a => a.Id == tag.Id);
            if (!isexist)
            {
                _logger.LogError("UpdateTag", $"{tag.Name} isimli_{tag.Id} Id'li Tag bulunamdı.");
                ModelState.AddModelError("", "Tag not found");
                return StatusCode(404, ModelState);
            }

            var result = await _tagRepo.Update(tag);
            if (!result)
            {
                _logger.LogError("UpdateTag_Fail", $"{tag.Name} isimli Tag güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Tag could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("UpdateTag_Success", $"{tag.Name} isimli_{tag.Id} id'li Tag güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteTag/{Id}")]
        public async Task<IActionResult> DeleteTag(int Id)
        {
            var property = await _tagRepo.Get(a => a.Id == Id);
            if (property == null)
            {
                _logger.LogError("DeleteTag", $"{Id} Id'li Tag bulunamdı.");
                ModelState.AddModelError("", "Tag not found");
                return StatusCode(404, ModelState);
            }

            var result = await _tagRepo.Delete(property);
            if (!result)
            {
                _logger.LogError("DeleteTag_Fail", $"{property.Name} isimli Referans silinirken hata oluştu.");
                ModelState.AddModelError("", "Tag could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("DeleteTag_Success", $"{property.Name} isimli Referans silindi.");
            return NoContent();
        }

    }
}
