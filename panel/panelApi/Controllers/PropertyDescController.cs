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
    [Route("api/propertyDesc")]
    [ApiController]
    public class PropertyDescController : ControllerBase
    {
        private readonly IPropertyDesRepo _propertyDesc;
        private readonly ILogger<PropertyDescController> _logger;

        public PropertyDescController(IPropertyDesRepo propertyDesc, ILogger<PropertyDescController> logger)
        {
            _propertyDesc = propertyDesc;
            _logger = logger;
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
                _logger.LogError("CreatePropertyDesc", "ÖzellikAçıklaması zaten mevcut");
                ModelState.AddModelError("", "PropertyDesc already exist");
                return StatusCode(404, ModelState);
            }
            var result = await _propertyDesc.Create(propertyDesc);
            if (result == null)
            {
                _logger.LogError("CreatePropertyDesc_Fail", $"{propertyDesc.Name} isimli ÖzellikAçıklaması oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "PropertyDesc could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("CreatePropertyDesc_Success", $"{propertyDesc.Name} isimli ÖzellikAçıklaması oluşturuldu.");
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
                _logger.LogError("GetPropertyDesc_Fail", $"{Id} Id'li ÖzellikAçıklaması bulunamdı.");
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
                _logger.LogError("GetAllPropertyDescs_Fail", "ÖzellikAçıklaması bulunamdı.");
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
        public async Task<IActionResult> UpdatePropertyDesc([FromBody] PropertyDescription propertyDesc)
        {
            var isexist = await _propertyDesc.IsExist(a => a.Id == propertyDesc.Id);
            if (!isexist)
            {
                _logger.LogError("UpdatePropertyDesc", $"{propertyDesc.Name} isimli_{propertyDesc.Id} Id'li ÖzellikAçıklaması bulunamdı.");
                ModelState.AddModelError("", "PropertyDesc not found");
                return StatusCode(404, ModelState);
            }

            var result =await _propertyDesc.Update(propertyDesc);
            if (!result)
            {
                _logger.LogError("UpdatePropertyDesc_Fail", $"{propertyDesc.Name} isimli ÖzellikAçıklaması güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "PropertyDesc could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("UpdatePropertyDesc_Success", $"{propertyDesc.Name} isimli_{propertyDesc.Id} id'li ÖzellikAçıklaması güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deletePropertyDesc/{Id}")]
        public async Task<IActionResult> DeletePropertyDesc(int Id)
        {
            var propertyDesc =await _propertyDesc.Get(a => a.Id == Id);
            if (propertyDesc==null)
            {
                _logger.LogError("DeletePropertyDesc", $"{Id} Id'li ÖzellikAçıklaması bulunamdı.");
                ModelState.AddModelError("", "PropertyDesc not found");
                return StatusCode(404, ModelState);
            }

            var result =await _propertyDesc.Delete(propertyDesc);
            if (!result)
            {
                _logger.LogError("DeletePropertyDesc_Fail", $"{propertyDesc.Name} isimli ÖzellikAçıklaması silinirken hata oluştu.");
                ModelState.AddModelError("", "PropertyDesc could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("DeletePropertyDesc_Success", $"{propertyDesc.Name} isimli ÖzellikAçıklaması silindi.");
            return NoContent();
        }
    }
}
