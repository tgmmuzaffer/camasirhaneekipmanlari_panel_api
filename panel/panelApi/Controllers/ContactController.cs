using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepo _contactRepo;
        public ContactController(IContactRepo contactRepo)
        {
            _contactRepo = contactRepo;
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
                ModelState.AddModelError("", "Contact already exist");
                return StatusCode(404, ModelState);
            }
            
            var result = await _contactRepo.Create(contact);
            if (result == null)
            {
                ModelState.AddModelError("", "Contact could not created");
                return StatusCode(500, ModelState);
            }
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
                ModelState.AddModelError("", "Contact not found");
                return StatusCode(404, ModelState);
            }
            var result = await _contactRepo.Update(contact);
            if (!result)
            {
                ModelState.AddModelError("", "Contact could not updated");
                return StatusCode(500, ModelState);
            }
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
                ModelState.AddModelError("", "Contact not found");
                return StatusCode(404, ModelState);
            }

            var result = await _contactRepo.Delete(contact);
            if (!result)
            {
                ModelState.AddModelError("", "Contact could not deleted");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
