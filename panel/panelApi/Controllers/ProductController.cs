using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.Repository.IRepository;
using System;
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
                _logger.LogError("CreateProduct__Ürün zaten mevcut");
                ModelState.AddModelError("", "Product already exist");
                return StatusCode(404, ModelState);
            }
            string filePath = _hostingEnvironment.ContentRootPath +"\\webpImages\\" + productDto.ImageName + ".webp";
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(productDto.ImagePath));
            product.CreateDate = productDto.CreateDate;
            product.Description = productDto.Description;
            product.IsPublish = productDto.IsPublish;
            product.Name = productDto.Name;
            product.ShortDesc = productDto.ShortDesc;
            product.ImagePath = productDto.ImageName + ".webp";
            product.CategoryId = productDto.CategoryId;
            var result = await _productRepo.Create(product);
            if (result == null)
            {
                _logger.LogError($"CreateProduct/Fail__{productDto.Name} isimli Ürün oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Product could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateProduct/Success__{productDto.Name} isimli Ürün oluşturuldu.");
            return Ok(product.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getProduct/{Id}")]
        public async Task<IActionResult> GetProduct(int Id)
        {
            var result = await _productRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetProduct/Fail__{Id} Id'li Ürün bulunamdı.");
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
                _logger.LogError("GetAllProducts/Fail__Ürünler bulunamdı.", "");
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
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDto productDto)
        {
            var orjprod = await _productRepo.Get(a => a.Id == productDto.Id);
            if (orjprod.Name == null)
            {
                _logger.LogError($"UpdateProduct__{productDto.Name} isimli_{productDto.Id} Id'li Ürün bulunamdı.");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            Product product = new()
            {
                CategoryId = productDto.CategoryId,
                CreateDate = productDto.CreateDate,
                Description = productDto.Description,
                Id = productDto.Id,
                ImagePath = productDto.ImageName + ".webp",
                IsPublish = productDto.IsPublish,
                Name = productDto.Name,
                ShortDesc = productDto.ShortDesc
            };
            if (product.ImagePath != orjprod.ImagePath)
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjprod.ImagePath;
                System.IO.File.Delete(imgpath);
            }
            
            var result = await _productRepo.Update(product);
            var t = 0;
            string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + productDto.ImageName + ".webp";
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(productDto.ImagePath));

            if (!result)
            {
                _logger.LogError($"UpdateProduct/Fail__{productDto.Name} isimli Ürün güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Product could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateProduct/Success__{productDto.Name} isimli_{productDto.Id} id'li Ürün güncellendi");
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
                _logger.LogError($"DeleteProduct__{Id} Id'li Ürün bulunamdı.");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + product.ImagePath;
            System.IO.File.Delete(imgpath);
            var result = await _productRepo.Delete(product);
            if (!result)
            {
                _logger.LogError($"DeleteProduct/Fail__{product.Name} isimli Ürün silinirken hata oluştu.");
                ModelState.AddModelError("", "Product could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteProduct/Success__{product.Name} isimli Ürün silindi.");
            return NoContent();
        }
    }
}
