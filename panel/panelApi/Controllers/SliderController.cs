using Microsoft.AspNetCore.Authorization;
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
    [Route("api/slider")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        private readonly ISliderRepo _sliderRepo;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SliderController(ISliderRepo sliderRepo, IHostingEnvironment hostingEnvironment)
        {
            _sliderRepo = sliderRepo;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Slider))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createSlider")]
        public async Task<IActionResult> CreateSlider([FromBody] SliderDto sliderDto)
        {
            Slider slider = new()
            {
                ButtonName = sliderDto.ButtonName,
                Content1 = sliderDto.Content1,
                Content2 = sliderDto.Content2,
                Content3 = sliderDto.Content3,
                Id = sliderDto.Id,
                ImageName = sliderDto.ImageName,
                IsShow = sliderDto.IsShow,
                Link = sliderDto.Link,
                SliderName = sliderDto.SliderName
            };
            var result = await _sliderRepo.Create(slider);
            string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + sliderDto.ImageName;
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(sliderDto.ImageData));
            return Ok(result.Id);
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Slider))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getSlider/{Id}")]
        public async Task<IActionResult> GetSlider(int Id)
        {
            var result = await _sliderRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                ModelState.AddModelError("", "Slider not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Slider))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllSliders")]
        public async Task<IActionResult> GetAllSliders()
        {
            var result = await _sliderRepo.GetList();

            if (result.Count < 0)
            {
                ModelState.AddModelError("", "Sliders not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Slider))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllSliders/{isshow}")]
        public async Task<IActionResult> GetAllSliders(bool isshow)
        {
            var result = await _sliderRepo.GetList(a => a.IsShow == isshow);

            if (result.Count < 0)
            {
                ModelState.AddModelError("", "Sliders not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateSlider")]
        public async Task<IActionResult> UpdateBlog([FromBody] SliderDto sliderDto)
        {
            var orjSlider = await _sliderRepo.Get(a => a.Id == sliderDto.Id);
            if (orjSlider.ImageName != sliderDto.ImageName) //
            {
                var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + orjSlider.ImageName;
                System.IO.File.Delete(imgpath);
            }

            string filePath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + sliderDto.ImageName;
            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(sliderDto.ImageData));
            Slider slider = new()
            {
                ButtonName = sliderDto.ButtonName,
                Content1 = sliderDto.Content1,
                Content2 = sliderDto.Content2,
                Content3 = sliderDto.Content3,
                Id = sliderDto.Id,
                ImageName = sliderDto.ImageName, //
                IsShow = sliderDto.IsShow,
                Link = sliderDto.Link,
                SliderName = sliderDto.SliderName
            };
            var result = await _sliderRepo.Update(slider);

            if (!result)
            {
                ModelState.AddModelError("", "Slider could not updated");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteSlider/{Id}")]
        public async Task<IActionResult> DeleteBlog(int Id)
        {
            var slider = await _sliderRepo.Get(a => a.Id == Id);
            var imgpath = _hostingEnvironment.ContentRootPath + "\\webpImages\\" + slider.ImageName;
            System.IO.File.Delete(imgpath);
            var result = await _sliderRepo.Delete(slider);

            if (!result)
            {
                ModelState.AddModelError("", "Slider could not deleted");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
