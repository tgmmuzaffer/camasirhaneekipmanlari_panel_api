using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IFeatureRepo _featureRepo;
        private readonly IFeatureDescriptionRepo _featureDescriptionRepo;
        private readonly IPr_Fe_RelRepo _pr_Fe_RelRepo;
        private readonly IPr_FeDesc_RelRepo _pr_FeDesc_RelRepo;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepo productRepo, 
            IFeatureRepo featureRepo, 
            IPr_Fe_RelRepo pr_Fe_RelRepo, 
            IPr_FeDesc_RelRepo pr_FeDesc_RelRepo,
            IFeatureDescriptionRepo featureDescriptionRepo,
            IWebHostEnvironment hostingEnvironment, 
            ILogger<ProductController> logger)
        {
            _productRepo = productRepo;
            _featureRepo = featureRepo;
            _featureDescriptionRepo = featureDescriptionRepo;
            _pr_Fe_RelRepo = pr_Fe_RelRepo;
            _pr_FeDesc_RelRepo = pr_FeDesc_RelRepo;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var isexist = await _productRepo.IsExist(a => a.Name == product.Name);
            if (isexist)
            {
                _logger.LogError("CreateProduct__Ürün zaten mevcut");
                ModelState.AddModelError("", "Product already exist");
                return StatusCode(404, ModelState);
            }
            string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + product.ImageName + ".webp";
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(product.ImagePath));
            product.ImagePath = product.ImageName + ".webp";
            var result = await _productRepo.Create(product);
            if (result == null)
            {
                _logger.LogError($"CreateProduct/Fail__{product.Name} isimli Ürün oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Product could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateProduct/Success__{product.Name} isimli Ürün oluşturuldu.");
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

            var feature_Rel = await _pr_Fe_RelRepo.GetFetureIdList(a => a.ProductId == result.Id);
            result.Feature = await _featureRepo.GetList(a => feature_Rel.Contains(a.Id));
            var featureDesc = await _pr_FeDesc_RelRepo.GetFeatureDescIdList(a => a.ProductId == result.Id);
            result.FeatureDescriptions = await _featureDescriptionRepo.GetList(a => featureDesc.Contains(a.Id));
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
            foreach (var item in result)
            {
                item.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetList(a => a.ProductId == item.Id);
                var feature_IdList = await _pr_Fe_RelRepo.GetFetureIdList(a => a.ProductId == item.Id);
                item.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetList(a => a.ProductId == item.Id);
                var featureDesc_IdList = await _pr_FeDesc_RelRepo.GetFeatureDescIdList(a => a.ProductId == item.Id);

                item.Feature = await _featureRepo.GetList(s => feature_IdList.Contains(s.Id));
                item.FeatureDescriptions = await _featureDescriptionRepo.GetList(f => featureDesc_IdList.Contains(f.Id));
            }
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
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            var orjprod = await _productRepo.Get(a => a.Id == product.Id);
            if (orjprod.Name == null)
            {
                _logger.LogError($"UpdateProduct__{product.Name} isimli_{product.Id} Id'li Ürün bulunamdı.");
                ModelState.AddModelError("", "Product not found");
                return StatusCode(404, ModelState);
            }

            product.ImagePath = product.ImageName + ".webp";
            if (product.ImagePath != orjprod.ImagePath)
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjprod.ImagePath;
                System.IO.File.Delete(imgpath);
            }

            if(product.Feature!=null  && product.FeatureDescriptions != null )
            {
                product.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetList(a => a.ProductId == product.Id);
                product.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetList(a => a.ProductId == product.Id);
            }

            var result = await _productRepo.Update(product);
            string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + product.ImageName + ".webp";
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(product.ImagePath));

            if (!result)
            {
                _logger.LogError($"UpdateProduct/Fail__{product.Name} isimli Ürün güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Product could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateProduct/Success__{product.Name} isimli_{product.Id} id'li Ürün güncellendi");
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

            product.Pr_Fe_Relationals = await _pr_Fe_RelRepo.GetList(a => a.ProductId == product.Id);
            product.Pr_FeDesc_Relationals = await _pr_FeDesc_RelRepo.GetList(a => a.ProductId == product.Id);
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
