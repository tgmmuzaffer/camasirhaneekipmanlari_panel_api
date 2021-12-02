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
    [Route("api/propertytab")]
    [ApiController]
    public class PropertyTabController : ControllerBase
    {
        private readonly IPropertyTabRepo _propertyTab;
        public PropertyTabController(IPropertyTabRepo propertyTab)
        {
            _propertyTab = propertyTab;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PropertyTab))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("CreateProperty")]
        public async Task<IActionResult> CreateProperty([FromBody] PropertyTab propertyTab)
        {
            var isexist = await _propertyTab.IsExist(a => a.Name == propertyTab.Name);
            if (isexist)
            {
                ModelState.AddModelError("", "Property already exist");
                return StatusCode(404, ModelState);
            }
            var result = await _propertyTab.Create(propertyTab);
            if (result == null)
            {
                ModelState.AddModelError("", "Property could not created");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute(nameof(GetProperty), new { propertyId = result.Id }, result);
        }

        [HttpGet("{propertyId:int}", Name = "GetProperty")]
        [ProducesResponseType(200, Type = typeof(PropertyTab))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProperty(int propertyId)
        {
            var result = _propertyTab.Get(a => a.Id == propertyId);
            if (result == null)
            {
                ModelState.AddModelError("", "Property not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PropertyTab))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetAllProperties")]
        public IActionResult GetAllProperties()
        {
            var result = _propertyTab.GetList();
            if (result == null)
            {
                ModelState.AddModelError("", "Property not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("UpdateProperty")]
        public async Task<IActionResult> UpdateProperty([FromBody] PropertyTab propertyTab)
        {
            var isexist = await _propertyTab.IsExist(a => a.Id == propertyTab.Id);
            if (!isexist)
            {
                ModelState.AddModelError("", "Property not found");
                return StatusCode(404, ModelState);
            }
            var result =await _propertyTab.Update(propertyTab);
            if (!result)
            {
                ModelState.AddModelError("", "Property could not updated");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("DeleteProperty")]
        public async Task<IActionResult> DeleteProperty(int Id)
        {
            var property =await _propertyTab.Get(a => a.Id == Id);
            if (property==null)
            {
                ModelState.AddModelError("", "Property not found");
                return StatusCode(404, ModelState);
            }

            var result =await _propertyTab.Delete(property);
            if (!result)
            {
                ModelState.AddModelError("", "property could not deleted");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
