using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly ILogger<CategoryController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMemoryCache _memoryCache;

        public CategoryController(ICategoryRepo categoryRepo, IWebHostEnvironment hostingEnvironment, ILogger<CategoryController> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _categoryRepo = categoryRepo;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;

        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            bool isOk = true;
            var isexist = await _categoryRepo.IsExist(a => a.Name == category.Name);
            if (isexist)
            {
                _logger.LogError("CreateCategory__Kategori zaten mevcut", "");
                ModelState.AddModelError("", "Category already exist");
                return StatusCode(404, ModelState);
            }

            if (!string.IsNullOrEmpty(category.ImagePath))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + category.ImageName;
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(category.ImagePath));
            }

            category.ImagePath = category.ImageName;
            var result = await _categoryRepo.Create(category);

            if (result == null && isOk == true)
            {
                _logger.LogError($"CreateCategory/Fail__{category.Name} isimli Kategori oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Category could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("CreateCategory_Success", $"{category.Name} isimli Kategori oluşturuldu.");
            return Ok(category.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getCategory/{Id}")]
        public async Task<IActionResult> GetCategory(int Id)
        {
            var result = await _categoryRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetCategory/Fail__{Id} Id'li Kategori bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getCategoryByName/{name}")]
        public async Task<IActionResult> GetCategory(string name)
        {
            var result = await _categoryRepo.Get(a => a.Name == name);
            if (result == null)
            {
                _logger.LogError($"GetCategory/Fail__{name} Isimli'li Kategori bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getCategoryName/{Id}")]
        public async Task<IActionResult> GetCategoryName(int Id)
        {
            var result = await _categoryRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError($"GetCategoryName/Fail__{Id} Id'li Kategori bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryRepo.GetList();
            if (result == null)
            {
                _logger.LogError("GetAllCategories/Fail__Kategoriler bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllCategoriesName")]
        public async Task<IActionResult> GetAllCategoriesName()
        {
            string key = "gac";
            var categories = new List<Category>();
            var ur = HttpContext.Request.GetDisplayUrl();
            if (ur.Contains("panel"))
            {
                categories = await _categoryRepo.GetNameList();
                if (categories == null)
                {
                    _logger.LogError("GetAllCategoriesName/Fail__Kategoriler bulunamdı.");
                    ModelState.AddModelError("", "Category not found");
                    return StatusCode(404, ModelState);
                }
            }
            else if (_memoryCache.TryGetValue(key, out categories))
            {
                return Ok(categories);

            }
            else
            {
                categories = await _categoryRepo.GetNameList();
                if (categories == null)
                {
                    _logger.LogError("GetAllCategoriesName/Fail__Kategoriler bulunamdı.");
                    ModelState.AddModelError("", "Category not found");
                    return StatusCode(404, ModelState);
                }

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _memoryCache.Set(key, categories, cacheExpiryOptions);
            }


            return Ok(categories);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            var orjcat = await _categoryRepo.Get(a => a.Id == category.Id);
            if (orjcat == null)
            {
                _logger.LogError($"UpdateCategory__{category.Name} isimli_{category.Id} Id'li Kategori bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }


            if ((orjcat.ImagePath != null) && (category.ImageName != orjcat.ImagePath))
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjcat.ImagePath;
                System.IO.File.Delete(imgpath);
            }

            if (!string.IsNullOrEmpty(category.ImagePath))
            {
                string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + category.ImageName;
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(category.ImagePath));
            }


            category.ImagePath = category.ImageName;
            var result = await _categoryRepo.Update(category);
            if (!result)
            {
                _logger.LogError($"UpdateCategory/Fail__{category.Name} isimli Kategori güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Category could not updated");
                return StatusCode(500, ModelState);
            }


            _logger.LogWarning($"UpdateCategory/Success__{category.Name} isimli_{category.Id} id'li Kategori güncellendi.");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteCategory/{Id}")]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            var category = await _categoryRepo.Get(a => a.Id == Id);
            if (category == null)
            {
                _logger.LogError($"DeleteCategory__{Id} Id'li Kategori bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + category.ImagePath;
            System.IO.File.Delete(imgpath);

            var result = await _categoryRepo.Delete(category);
            if (!result)
            {
                _logger.LogError($"DeleteCategory/Fail__{category.Name} başlıklı Kategori silinirken hata oluştu.");
                ModelState.AddModelError("", "Category could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteCategory/Success__{category.Name} başlıklı Kategori silindi.");
            return NoContent();
        }
    }
}
