﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IBlogRepo _blogRepo;
        private readonly IBlogTagRepo _blogTagRepo;
        private readonly ITagRepo _tagRepo;
        public BlogController(IBlogRepo blogRepo, IHostingEnvironment hostingEnvironment, IBlogTagRepo blogTagRepo, ITagRepo tagRepo)
        {
            _blogRepo = blogRepo;
            _hostingEnvironment = hostingEnvironment;
            _blogTagRepo = blogTagRepo;
            _tagRepo = tagRepo;
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
                ModelState.AddModelError("", "Blog already exist");
                return StatusCode(404, ModelState);
            }
            string filePath = _hostingEnvironment.ContentRootPath + @"\webpImages\" + blogDto.ImageName + ".webp";
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

            if (result == null || blogTagResult == null)
            {
                ModelState.AddModelError("", "Blog could not created");
                return StatusCode(500, ModelState);
            }
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
            if (blogTagResult.Count > 0)
            {
                var tagList = await _tagRepo.GetList(a => blogTagResult.Contains(a.Id));
                blogDto.TagIds = tagList.Select(a => a.Id).ToList();
                blogDto.TagNames = tagList.Select(a => a.Name).ToList();
            }
            if (result == null)
            {
                ModelState.AddModelError("", "Blog not found");
                return StatusCode(404, ModelState);
            }
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
                if (blogTagResult.Count > 0)
                {
                    var tagList = await _tagRepo.GetList(a => blogTagResult.Contains(a.Id));
                    blogDto.TagIds = tagList.Select(a => a.Id).ToList();
                    blogDto.TagNames = tagList.Select(a => a.Name).ToList();
                }
                blogDtos.Add(blogDto);
            }

            if (result.Count < 0 || blogDtos.Count < 0)
            {
                ModelState.AddModelError("", "Blog not found");
                return StatusCode(404, ModelState);
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
            try
            {
                var orjblog = await _blogRepo.Get(a => a.Id == blogDto.Id);
                bool isblogTagupdated = true;
                if (orjblog.Title==null)
                {
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
                if (!result || !isblogTagupdated)
                {
                    ModelState.AddModelError("", "Blog could not updated");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteBlog/{Id}")]
        public async Task<IActionResult> DeleteBlog(int Id)
        {
            try
            {
                var blog = await _blogRepo.Get(a => a.Id == Id);
                if (blog == null)
                {
                    ModelState.AddModelError("", "Blog not found");
                    return StatusCode(404, ModelState);
                }
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + blog.ImagePath;
                System.IO.File.Delete(imgpath);
                var result = await _blogRepo.Delete(blog);

                var blogtag = await _blogTagRepo.RemoveMultiple(Id);

                if (!result || !blogtag)
                {
                    ModelState.AddModelError("", "Blog could not deleted");
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return NoContent();
        }
    }
}