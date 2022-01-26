using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepo _tagRepo;
        public TagController(ITagRepo tagRepo)
        {
            _tagRepo = tagRepo;
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
                ModelState.AddModelError("", "Tag already exist");
                return StatusCode(404, ModelState);
            }
            var result = await _tagRepo.Create(tag);
            if (result == null)
            {
                ModelState.AddModelError("", "Tag could not created");
                return StatusCode(500, ModelState);
            }
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
                ModelState.AddModelError("", "Tag not found");
                return StatusCode(404, ModelState);
            }
            var result = await _tagRepo.Update(tag);
            if (!result)
            {
                ModelState.AddModelError("", "Tag could not updated");
                return StatusCode(500, ModelState);
            }
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
                ModelState.AddModelError("", "Tag not found");
                return StatusCode(404, ModelState);
            }

            var result = await _tagRepo.Delete(property);
            if (!result)
            {
                ModelState.AddModelError("", "Tag could not deleted");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
