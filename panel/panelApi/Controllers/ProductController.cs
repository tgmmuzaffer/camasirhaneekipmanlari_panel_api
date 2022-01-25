using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _productRepo;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductController(IProductRepo productRepo, IHostingEnvironment hostingEnvironment)
        {
            _productRepo = productRepo;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            Product product = new Product();
            var isexist = await _productRepo.IsExist(a => a.Name == productDto.Name);
            if (isexist)
            {
                ModelState.AddModelError("", "Product already exist");
                return StatusCode(404, ModelState);
            }
            string p = _hostingEnvironment.ContentRootPath;
            string filePath = _hostingEnvironment.ContentRootPath + @"\webpImages\" + productDto.ImageName + ".webp";
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(productDto.ImagePath));
            product.CreateDate = productDto.CreateDate;
            product.Description = productDto.Description;
            product.IsPublish = productDto.IsPublish;
            product.Name = productDto.Name;
            product.ShortDesc = productDto.ShortDesc;
            product.ImagePath = productDto.ImageName + ".webp";
            var result = await _productRepo.Create(product);
            if (result == null)
            {
                ModelState.AddModelError("", "Product could not created");
                return StatusCode(500, ModelState);
            }
            return Ok(product.Id);
        }

        [AllowAnonymous]
        [HttpGet("{productId:int}", Name = "getProduct")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var result = await _productRepo.Get(a => a.Id == productId);
            if (result == null)
            {
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productRepo.GetList();
            if (result.Count < 0)
            {
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            var isexist = await _productRepo.IsExist(a => a.Id == product.Id);
            if (!isexist)
            {
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }
            var result = await _productRepo.Update(product);
            if (!result)
            {
                ModelState.AddModelError("", "Product could not updated");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteProduct/{Id}")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var product = await _productRepo.Get(a => a.Id == Id);
            if (product == null)
            {
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            var result = await _productRepo.Delete(product);
            if (!result)
            {
                ModelState.AddModelError("", "Product could not deleted");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
