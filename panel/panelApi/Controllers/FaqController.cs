using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/faq")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly ILogger<FaqController> _logger;
        private readonly IFaqRepo _faqRepo;
        private readonly IMemoryCache _memoryCache;

        public FaqController(IFaqRepo faqRepo, ILogger<FaqController> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _faqRepo = faqRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Faq))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createFaq")]
        public async Task<IActionResult> CreateFaq([FromBody] Faq faq)
        {
            var isexist = await _faqRepo.IsExist(a => a.Title == faq.Title);
            if (isexist)
            {
                _logger.LogError("CreateFaq__Title Bilgisi zaten mevcut");
                ModelState.AddModelError("", "Faq already exist");
                return StatusCode(404, ModelState);
            }

            var result = await _faqRepo.Create(faq);
            if (result == null)
            {
                _logger.LogError($"CreateFaq/Fail__{faq.Title} isimli Faq bilgisi oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Faq could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"CreateFaq/Success__{faq.Title} isimli Faq Bilgisi oluşturuldu.");
            return Ok(faq.Id);
        }


        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Faq))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getFaq/{id}")]
        public async Task<IActionResult> GetFaq(int id)
        {
            var faq = new Faq();
            faq = await _faqRepo.Get(a=>a.Id==id);
            if (faq == null)
            {
                _logger.LogError($"GetFaq/Fail__ Id'li Faq Bilgisi bulunamdı.");
                ModelState.AddModelError("", "Faq not found");
                return StatusCode(404, ModelState);
            }

            return Ok(faq);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Faq))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllFaqs")]
        public async Task<IActionResult> GetAllFaqs()
        {
            var result = await _faqRepo.GetListWithRelatedEntity();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllFaqs/Fail__Faq Bilgileri bulunamdı.");
                ModelState.AddModelError("", "Faq not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateFaq")]
        public async Task<IActionResult> UpdateFaq([FromBody] Faq faq)
        {
            var isexist = await _faqRepo.IsExist(a => a.Id == faq.Id);
            if (!isexist)
            {
                _logger.LogError($"UpdateFaq__{faq.Title} isimli_{faq.Id} Id'li faq Bilgisi bulunamdı.");
                ModelState.AddModelError("", "Faq not found");
                return StatusCode(404, ModelState);
            }
            var result = await _faqRepo.Update(faq);
            if (!result)
            {
                _logger.LogError($"UpdateFaq/Fail__{faq.Title} isimli faq Bilgisi güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Faq could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateFaq/Success_{faq.Title} isimli_{faq.Id} id'li faq Bilgisi güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteFaq/{id}")]
        public async Task<IActionResult> DeleteFaq(int Id)
        {
            var faq = await _faqRepo.Get(a => a.Id == Id);
            if (faq == null)
            {
                _logger.LogError($"DeleteFaq__{Id} Id'li İletişim Bilgisi bulunamdı.");
                ModelState.AddModelError("", "Faq not found");
                return StatusCode(404, ModelState);
            }

            var result = await _faqRepo.Delete(faq);
            if (!result)
            {
                _logger.LogError($"DeleteFaq/Fail__{faq.Title} isimli Faq Bilgisi silinirken hata oluştu.");
                ModelState.AddModelError("", "Faq could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteFaq/Success__{faq.Title} isimli Faq Bilgisi silindi.");
            return NoContent();
        }
    }
}
