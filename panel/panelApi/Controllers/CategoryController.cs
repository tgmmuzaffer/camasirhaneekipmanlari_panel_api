using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.Repository.IRepository;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly IPropertyCategoryRepo _propertyCategoryRepo;
        private readonly IProductPropertyRepo _productPropertyRepo;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryRepo categoryRepo, IPropertyCategoryRepo propertyCategoryRepo, IProductPropertyRepo productPropertyRepo, ILogger<CategoryController> logger)
        {
            _categoryRepo = categoryRepo;
            _propertyCategoryRepo = propertyCategoryRepo;
            _productPropertyRepo = productPropertyRepo;
            _logger = logger;
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            bool isOk = true;
            var isexist = await _categoryRepo.IsExist(a => a.Name == categoryDto.Name);
            if (isexist)
            {
                _logger.LogError("CreateCategory", "Kategori zaten mevcut");
                ModelState.AddModelError("", "Category already exist");
                return StatusCode(404, ModelState);
            }

            Category category = new Category()
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name
            };
            var result = await _categoryRepo.Create(category);
            for (int i = 0; i < categoryDto.ProductPropertyIds.Count; i++)
            {
                PropertyCategory propertyCategory = new PropertyCategory()
                {
                    CategoryId = result.Id,
                    ProductPropertyId = categoryDto.ProductPropertyIds[i]
                };

                var res = await _propertyCategoryRepo.Create(propertyCategory);
                isOk = res == null ? false : true;
            }

            if (result == null && isOk == true)
            {
                _logger.LogError("CreateCategory_Fail", $"{categoryDto.Name} isimli Kategori oluşturulurken hata meydana geldi.");
                _logger.LogError("CreateCategory_Fail", $"{categoryDto.Name} isimli Kategori oluşturulurken Özellikleri eklenemedi.");
                ModelState.AddModelError("", "Category could not created");              
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("CreateCategory_Success", $"{categoryDto.Name} isimli Kategori oluşturuldu.");
            return Ok(categoryDto.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getCategory/{Id}")]
        public async Task<IActionResult> GetCategory(int Id)
        {
            var result = await _categoryRepo.Get(a => a.Id == Id);
            var resultPropertyCategory = await _propertyCategoryRepo.GetIdList(b => b.CategoryId == Id);
            if (resultPropertyCategory == null)
            {
                _logger.LogError("GetCategory_Fail", $"{Id} Id'li Kategori Özelliği bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            var productProperties = await _productPropertyRepo.GetNames(d => resultPropertyCategory.Contains(d.Id));
            if (productProperties == null)
            {
                _logger.LogError("GetCategory_Fail", $"{Id} Id'li Ürün Özelliği bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            CategoryDto categoryDto = new()
            {
                Id = result.Id,
                Name = result.Name,
                ProductPropertyNames = productProperties,
                ProductPropertyIds= resultPropertyCategory
            };
            if (result == null)
            {
                _logger.LogError("GetCategory_Fail", $"{Id} Id'li Kategori bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            return Ok(categoryDto);
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
                _logger.LogError("GetAllCategories_Fail", "Kategoriler bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto categoryDto)
        {
            bool isOk = true;
            var isexist = await _categoryRepo.IsExist(a => a.Id == categoryDto.Id);
            if (!isexist)
            {
                _logger.LogError("UpdateCategory", $"{categoryDto.Name} isimli_{categoryDto.Id} Id'li Kategori bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }
            Category category = new Category()
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name
            };
            var result = await _categoryRepo.Update(category);
            if (!isexist)
            {
                _logger.LogError("UpdateCategory_Fail", $"{categoryDto.Name} isimli Kategori güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Category could not updated");
                return StatusCode(500, ModelState);
            }

            for (int i = 0; i < categoryDto.ProductPropertyIds.Count; i++)
            {
                PropertyCategory propertyCategory = new PropertyCategory()
                {
                    CategoryId = categoryDto.Id,
                    ProductPropertyId = categoryDto.ProductPropertyIds[i]
                };

                var res = await _propertyCategoryRepo.Update(propertyCategory);
                isOk = res == false ? false : true;
            }
            if (!result || !isOk)
            {
                _logger.LogError("UpdateCategory_Fail", $"{categoryDto.Name} isimli Kategori güncellenirken Özellikleri eklenemedi.");
                ModelState.AddModelError("", "Category could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("UpdateCategory_Success", $"{categoryDto.Name} isimli_{categoryDto.Id} id'li Kategori güncellendi");
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
                _logger.LogError("DeleteCategory", $"{Id} Id'li Kategori bulunamdı.");
                ModelState.AddModelError("", "Category not found");
                return StatusCode(404, ModelState);
            }

            var result = await _categoryRepo.Delete(category);
            if (!result)
            {
                _logger.LogError("DeleteCategory_Fail", $"{category.Name} başlıklı Kategori silinirken hata oluştu.");
                ModelState.AddModelError("", "Category could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("DeleteCategory_Success", $"{category.Name} başlıklı Kategori silindi.");
            return NoContent();
        }
    }
}
