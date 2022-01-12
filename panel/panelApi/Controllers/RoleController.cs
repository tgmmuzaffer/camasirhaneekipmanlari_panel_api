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
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepo _roleRepo;
        public RoleController(IRoleRepo roleRepo)
        {
            _roleRepo=roleRepo;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Role))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result =await _roleRepo.GetRoles();
            if (result.Count < 0)
            {
                ModelState.AddModelError("", "Property not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Role))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getRole/{Id}")]
        public async Task<IActionResult> GetRole(int Id)
        {
            var result = await _roleRepo.GetRole();
            if (result!=null)
            {
                ModelState.AddModelError("", "Property not found");
                return StatusCode(404, ModelState);
            }
            return Ok(result);
        }
    }
}
