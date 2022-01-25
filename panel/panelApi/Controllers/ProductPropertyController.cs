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
    [Route("api/productproperty")]
    [ApiController]
    public class ProductPropertyController : ControllerBase
    {
        private readonly IProductPropertyRepo _productPropertyRepo;
        private readonly IPropertyCategoryRepo _propertyCategoryRepo;
        public ProductPropertyController(IProductPropertyRepo productPropertyRepo, IPropertyCategoryRepo propertyCategoryRepo)
        {
            _productPropertyRepo = productPropertyRepo;
            _propertyCategoryRepo = propertyCategoryRepo;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ProductProperty))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createProductProperty")]
        public async Task<IActionResult> CreateProperty([FromBody] ProductProperty productProperty)
        {
            var isexist = await _productPropertyRepo.IsExist(a => a.Name == productProperty.Name);
            if (isexist)
            {
                ModelState.AddModelError("", "ProductProperty already exist");
                return StatusCode(404, ModelState);
            }
            var result = await _productPropertyRepo.Create(productProperty);
            if (result == null)
            {
                ModelState.AddModelError("", "ProductProperty could not created");
                return StatusCode(500, ModelState);
            }
            return Ok(productProperty.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ProductProperty))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getProductProperty/{Id}")]
        public async Task<IActionResult> GetProductProperty(int Id)
        {
            var result = await _productPropertyRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                ModelState.AddModelError("", "ProductProperty not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ProductProperty))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllProductProperties")]
        public async Task<IActionResult> GetAllProductProperties()
        {
            var result = await _productPropertyRepo.GetList();
            if (result.Count < 0)
            {
                ModelState.AddModelError("", "ProductProperty not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateProductProperty")]
        public async Task<IActionResult> UpdateProductProperty([FromBody] ProductProperty productProperty)
        {
            var isexist = await _productPropertyRepo.IsExist(a => a.Id == productProperty.Id);
            if (!isexist)
            {
                ModelState.AddModelError("", "ProductProperty not found");
                return StatusCode(404, ModelState);
            }
            var result = await _productPropertyRepo.Update(productProperty);
            if (!result)
            {
                ModelState.AddModelError("", "ProductProperty could not updated");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteProductProperty/{Id}")]
        public async Task<IActionResult> DeleteProductProperty(int Id)
        {
            var productProperty = await _productPropertyRepo.Get(a => a.Id == Id);
            if (productProperty == null)
            {
                ModelState.AddModelError("", "ProductProperty not found");
                return StatusCode(404, ModelState);
            }
            var result = await _productPropertyRepo.Delete(productProperty);
            var productCategory = _propertyCategoryRepo.RemoveMultiple(Id);
            if (!result)
            {
                ModelState.AddModelError("", "ProductProperty could not deleted");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


    }
}
