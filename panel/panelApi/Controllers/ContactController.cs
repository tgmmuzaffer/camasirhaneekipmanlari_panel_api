using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IContactRepo _contactRepo;
        public ContactController(IContactRepo contactRepo, ILogger<ContactController> logger)
        {
            _contactRepo = contactRepo;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Contact))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [Route("createContact")]
        public async Task<IActionResult> CreateContact([FromBody] Contact contact)
        {
            var isexist = await _contactRepo.IsExist(a => a.MapAdress == contact.MapAdress);
            if (isexist)
            {
                _logger.LogError("CreateContact", "İletişim Bilgisi zaten mevcut");
                ModelState.AddModelError("", "Contact already exist");
                return StatusCode(404, ModelState);
            }
            
            var result = await _contactRepo.Create(contact);
            if (result == null)
            {
                _logger.LogError("CreateContact_Fail", $"{contact.Name} isimli İletişim bilgisi oluşturulurken hata meydana geldi.");
                ModelState.AddModelError("", "Contact could not created");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("CreateContact_Success", $"{contact.Name} isimli İletişim Bilgisi oluşturuldu.");
            return Ok(contact.Id);
        }


        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Contact))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getContact/{Id}")]
        public async Task<IActionResult> GetContact(int Id)
        {
            var result = await _contactRepo.Get(a => a.Id == Id);
            if (result == null)
            {
                _logger.LogError("GetContact_Fail", $"{Id} Id'li İletişim Bilgisi bulunamdı.");
                ModelState.AddModelError("", "Contact not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Contact))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllContacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            var result = await _contactRepo.GetList();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllContacts_Fail", "İletişim Bilgileri bulunamdı.");
                ModelState.AddModelError("", "Contact not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateContact")]
        public async Task<IActionResult> UpdateContact([FromBody] Contact contact)
        {
            var isexist = await _contactRepo.IsExist(a => a.Id == contact.Id);
            if (!isexist)
            {
                _logger.LogError("UpdateContact", $"{contact.Name} isimli_{contact.Id} Id'li İletişim Bilgisi bulunamdı.");
                ModelState.AddModelError("", "Contact not found");
                return StatusCode(404, ModelState);
            }
            var result = await _contactRepo.Update(contact);
            if (!result)
            {
                _logger.LogError("UpdateContact_Fail", $"{contact.Name} isimli İletişim Bilgisi güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "Contact could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("UpdateContact_Success", $"{contact.Name} isimli_{contact.Id} id'li İletişim Bilgisi güncellendi");
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteContact/{Id}")]
        public async Task<IActionResult> DeleteContact(int Id)
        {
            var contact = await _contactRepo.Get(a => a.Id == Id);
            if (contact == null)
            {
                _logger.LogError("DeleteContact", $"{Id} Id'li İletişim Bilgisi bulunamdı.");
                ModelState.AddModelError("", "Contact not found");
                return StatusCode(404, ModelState);
            }

            var result = await _contactRepo.Delete(contact);
            if (!result)
            {
                _logger.LogError("DeleteCategory_Fail", $"{contact.Name} isimli İletişim Bilgisi silinirken hata oluştu.");
                ModelState.AddModelError("", "Contact could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning("DeleteCategory_Success", $"{contact.Name} isimli İletişim Bilgisi silindi.");
            return NoContent();
        }
    }
}
