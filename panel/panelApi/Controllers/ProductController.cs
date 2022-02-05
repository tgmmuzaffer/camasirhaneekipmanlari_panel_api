using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepo productRepo, IHostingEnvironment hostingEnvironment, ILogger<ProductController> logger)
        {
            _productRepo = productRepo;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
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
                _logger.LogError("CreateProduct", "Ürün zaten mevcut");
                ModelState.AddModelError("", "Product already exist");
                return StatusCode(404, ModelState);
            }
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
                _logger.LogError("CreateProduct_Fail", $"{productDto.Name} isimli Ürün oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Product could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("CreateProduct_Success", $"{productDto.Name} isimli Ürün oluşturuldu.");
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
                _logger.LogError("GetProduct_Fail", $"{productId} Id'li Ürün bulunamdı.");
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
                _logger.LogError("GetAllProducts_Fail", "Ürünler bulunamdı.");
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
                _logger.LogError("UpdateProduct", $"{product.Name} isimli_{product.Id} Id'li Ürün bulunamdı.");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }
            var result = await _productRepo.Update(product);
            if (!result)
            {
                _logger.LogError("UpdateProduct_Fail", $"{product.Name} isimli Ürün güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Product could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("UpdateProduct_Success", $"{product.Name} isimli_{product.Id} id'li Ürün güncellendi");
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
                _logger.LogError("DeleteProduct", $"{Id} Id'li Ürün bulunamdı.");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            var result = await _productRepo.Delete(product);
            if (!result)
            {
                _logger.LogError("DeleteProduct_Fail", $"{product.Name} isimli Ürün silinirken hata oluştu.");
                ModelState.AddModelError("", "Product could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("DeleteProduct_Success", $"{product.Name} isimli Ürün silindi.");
            return NoContent();
        }
    }
}
