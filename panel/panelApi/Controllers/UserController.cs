using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using panelApi.Models;
using panelApi.Repository.IRepository;

namespace panelApi.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody] User user)
        {
            var userdata = _userRepo.Authenticate(user.UserName, user.Password);
            if(userdata == null)
            {
                return BadRequest(new { message = "UserName or Password is incorrect" });
            }
            return Ok(userdata);
        }
    }
}
