using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IBlogRepo _blogRepo;
        private readonly IBlogTagRepo _blogTagRepo;
        private readonly ITagRepo _tagRepo;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogRepo blogRepo, IWebHostEnvironment hostingEnvironment, IBlogTagRepo blogTagRepo, ITagRepo tagRepo, ILogger<BlogController> logger)
        {
            _blogRepo = blogRepo;
            _hostingEnvironment = hostingEnvironment;
            _blogTagRepo = blogTagRepo;
            _tagRepo = tagRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Blog))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createBlog")]
        public async Task<IActionResult> CreateBlog([FromBody] BlogDto blogDto)
        {
            var isexist = await _blogRepo.IsExist(a => a.Title == blogDto.Title);
            if (isexist)
            {
                _logger.LogError("CreateReferance___Blog zaten mevcut");
                ModelState.AddModelError("", "Blog already exist");
                return StatusCode(404, ModelState);
            }
            string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + blogDto.ImageName + ".webp";
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(blogDto.ImagePath));
            Blog blog = new()
            {
                Content = blogDto.Content,
                CreateDate = blogDto.CreateDate,
                Id = blogDto.Id,
                ImagePath = blogDto.ImageName + ".webp",
                ShortDesc = blogDto.ShortDesc,
                Title = blogDto.Title
            };
            var result = await _blogRepo.Create(blog);
            if (result == null)
            {
                _logger.LogError($"CreateBlog/Fail__{blogDto.Title} başlıklı blog oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Blog could not created");
                return StatusCode(500, ModelState);
            }
            List<BlogTag> blogTags = new();
            foreach (var item in blogDto.TagIds)
            {
                BlogTag blogTag = new BlogTag
                {
                    BlogId = result.Id,
                    TagId = item
                };
                blogTags.Add(blogTag);
            }
            var blogTagResult = await _blogTagRepo.AddList(blogTags);

            if (blogTagResult == null)
            {
                _logger.LogError($"CreateBlog/Fail__{blogDto.Title} başlıklı blog oluşturulurken taglar eklenemedi.");
                ModelState.AddModelError("", "Blog could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateBlog/Success__{result.Title} başlıklı blog oluşturuldu");
            return Ok(blog.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Blog))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getBlog/{Id}")]
        public async Task<IActionResult> GetBlog(int Id)
        {
            var result = await _blogRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogWarning($"GetBlog__{result.Title} başlıklı {Id} id'li blog bulunamadı.");
                ModelState.AddModelError("", "Blog not found");
                return StatusCode(404, ModelState);
            }

            BlogDto blogDto = new();
            blogDto.Content = result.Content;
            blogDto.CreateDate = result.CreateDate;
            blogDto.Id = result.Id;
            blogDto.ImagePath = result.ImagePath;
            blogDto.ShortDesc = result.ShortDesc;
            blogDto.TagIds = null;
            blogDto.TagNames = null;
            blogDto.Title = result.Title;
            var blogTagResult = await _blogTagRepo.GetIdList(a => a.BlogId == result.Id);
            if (blogTagResult != null)
            {
                _logger.LogWarning($"GetBlog__{Id} id'li blog a ait taglar bulunamadı.");
                ModelState.AddModelError("", "BlogTag not found");
                return StatusCode(404, ModelState);
            }

            var tagList = await _tagRepo.GetList(a => blogTagResult.Contains(a.Id));
            if (blogTagResult != null)
            {
                _logger.LogWarning($"GetBlog__{result.Title} başlıklı {Id} id'li blog a ait taglar bulunamadı.");
                ModelState.AddModelError("", "BlogTag not found");
                return StatusCode(404, ModelState);
            }

            blogDto.TagIds = tagList.Select(a => a.Id).ToList();
            blogDto.TagNames = tagList.Select(a => a.Name).ToList();
            return Ok(blogDto);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(BlogDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllBlogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            List<BlogDto> blogDtos = new();
            var result = await _blogRepo.GetList();
            if (result.Count < 0)
            {
                _logger.LogWarning("GetAllBlogs__Bloglar bulunamadı.");
                ModelState.AddModelError("", "Blogs not found");
                return StatusCode(404, ModelState);
            }

            foreach (var item in result)
            {
                BlogDto blogDto = new();
                blogDto.Content = item.Content;
                blogDto.CreateDate = item.CreateDate;
                blogDto.Id = item.Id;
                blogDto.ImagePath = item.ImagePath;
                blogDto.ShortDesc = item.ShortDesc;
                blogDto.TagIds = null;
                blogDto.TagNames = null;
                blogDto.Title = item.Title;
                var blogTagResult = await _blogTagRepo.GetIdList(a => a.BlogId == item.Id);
                if (blogTagResult == null)
                {
                    _logger.LogWarning("GetAllBlogs__Bloglara ait taglar bulunamadı.");
                    ModelState.AddModelError("", "BlogTag not found");
                    return StatusCode(404, ModelState);
                }

                var tagList = await _tagRepo.GetList(a => blogTagResult.Contains(a.Id));
                blogDto.TagIds = tagList.Select(a => a.Id).ToList();
                blogDto.TagNames = tagList.Select(a => a.Name).ToList();
                blogDtos.Add(blogDto);
            }

            return Ok(blogDtos);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateBlog")]
        public async Task<IActionResult> UpdateBlog([FromBody] BlogDto blogDto)
        {
            var orjblog = await _blogRepo.Get(a => a.Id == blogDto.Id);
            bool isblogTagupdated = true;
            if (orjblog.Title == null)
            {
                _logger.LogError($"UpdateSlider__{blogDto.Title} başlıklı {blogDto.Id} id'li blog bulunamadı.");
                ModelState.AddModelError("", "Blog not found");
                return StatusCode(404, ModelState);
            }

            Blog blog = new()
            {
                Content = blogDto.Content,
                Id = blogDto.Id,
                CreateDate = blogDto.CreateDate,
                ImagePath = blogDto.ImageName + ".webp",
                ShortDesc = blogDto.ShortDesc,
                Title = blogDto.Title
            };
            if (blog.ImagePath != orjblog.ImagePath)
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjblog.ImagePath;
                System.IO.File.Delete(imgpath);
            }

            var result = await _blogRepo.Update(blog);
            if (!result)
            {
                _logger.LogError($"UpdateBlog/Fail__{blogDto.Title} başlıklı blog güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Blog could not updated");
                return StatusCode(500, ModelState);
            }

            foreach (var item in blogDto.TagIds)
            {
                BlogTag blogTag = new BlogTag
                {
                    BlogId = blogDto.Id,
                    TagId = item
                };
                isblogTagupdated = await _blogTagRepo.Update(blogTag);
            }
            string filePath = _hostingEnvironment.ContentRootPath + @"\webpImages\" + blogDto.ImageName + ".webp";
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(blogDto.ImagePath));
            if (!isblogTagupdated)
            {
                _logger.LogError($"UpdateBlog/Fail__{blogDto.Title} başlıklı blog güncellenirken taglar eklenemedi.");
                ModelState.AddModelError("", "Blog could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateBlog/Success__{blogDto.Title} başlıklı_{blogDto.Id} id'li blog güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteBlog/{Id}")]
        public async Task<IActionResult> DeleteBlog(int Id)
        {
            var blog = await _blogRepo.Get(a => a.Id == Id);
            if (blog == null)
            {
                _logger.LogError($"DeleteSlider__{blog.Id} id'li blog bulunamadı.");
                ModelState.AddModelError("", "Blog not found");
                return StatusCode(404, ModelState);
            }
            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + blog.ImagePath;
            System.IO.File.Delete(imgpath);
            var result = await _blogRepo.Delete(blog);
            if (!result)
            {
                _logger.LogError($"DeleteBlog/Fail__{blog.Title} başlıklı blog silinirken hata oluştu.");
                ModelState.AddModelError("", "Blog could not deleted");
                return StatusCode(500, ModelState);
            }

            var blogtag = await _blogTagRepo.RemoveMultiple(Id);
            if (!blogtag)
            {
                _logger.LogError($"DeleteBlog/Fail__{blog.Title} başlıklı blogun tagları silinirken hata oluştu..");
                ModelState.AddModelError("", "Blog could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteBlog/Succeess__{blog.Title} başlıklı blog silindi.");
            return NoContent();
        }
    }
}
