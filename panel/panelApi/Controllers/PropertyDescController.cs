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
    [Route("api/propertyDesc")]
    [ApiController]
    public class PropertyDescController : ControllerBase
    {
        private readonly IPropertyDesRepo _propertyDesc;
        public PropertyDescController(IPropertyDesRepo propertyDesc)
        {
            _propertyDesc = propertyDesc;
        }

        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PropertyDescription))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createPropertyDesc")]
        public async Task<IActionResult> CreatePropertyDesc([FromBody] PropertyDescription propertyDesc)
        {
            var isexist = await _propertyDesc.IsExist(a => a.Name == propertyDesc.Name);
            if (isexist)
            {
                ModelState.AddModelError("", "PropertyDesc already exist");
                return StatusCode(404, ModelState);
            }
            var result = await _propertyDesc.Create(propertyDesc);
            if (result == null)
            {
                ModelState.AddModelError("", "PropertyDesc could not created");
                return StatusCode(500, ModelState);
            }
            return Ok(propertyDesc.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(PropertyDescription))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getPropertyDesc/{Id}")]
        public async Task<IActionResult> GetPropertyDesc(int Id)
        {
            var result =await _propertyDesc.Get(a => a.Id == Id);
            if (result == null)
            {
                ModelState.AddModelError("", "PropertyDesc not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PropertyDescription))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllPropertyDescs")]
        public async Task<IActionResult> GetAllPropertyDescs()
        {
            var result =await _propertyDesc.GetList();
            if (result.Count<0)
            {
                ModelState.AddModelError("", "PropertyDesc not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updatePropertyDesc")]
        public async Task<IActionResult> UpdateProperty([FromBody] PropertyDescription propertyDesc)
        {
            var isexist = await _propertyDesc.IsExist(a => a.Id == propertyDesc.Id);
            if (!isexist)
            {
                ModelState.AddModelError("", "PropertyDesc not found");
                return StatusCode(404, ModelState);
            }
            var result =await _propertyDesc.Update(propertyDesc);
            if (!result)
            {
                ModelState.AddModelError("", "PropertyDesc could not updated");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deletePropertyDesc/{Id}")]
        public async Task<IActionResult> DeleteProperty(int Id)
        {
            var property =await _propertyDesc.Get(a => a.Id == Id);
            if (property==null)
            {
                ModelState.AddModelError("", "PropertyDesc not found");
                return StatusCode(404, ModelState);
            }

            var result =await _propertyDesc.Delete(property);
            if (!result)
            {
                ModelState.AddModelError("", "PropertyDesc could not deleted");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
