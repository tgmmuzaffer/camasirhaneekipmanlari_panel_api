using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepo _roleRepo;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleRepo roleRepo, ILogger<RoleController> logger)
        {
            _roleRepo = roleRepo;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Role))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleRepo.GetRoles();
            if (result.Count < 0)
            {
                _logger.LogError("GetAllRoles/Fail__Roller bulunamdı.");
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
            if (result != null)
            {
                _logger.LogError($"GetRole/Fail__{Id} Id'li Rol bulunamdı.");
                ModelState.AddModelError("", "Property not found");
                return StatusCode(404, ModelState);
            }

            return Ok(result);
        }
    }
}
