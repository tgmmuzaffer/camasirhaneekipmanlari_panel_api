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
            if (userdata == null)
            {
                return BadRequest(new { message = "UserName or Password is incorrect" });
            }
            return Ok(userdata);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public IActionResult Register(string mail, string password)
        {
            bool isexist = _userRepo.IsUnique(mail);
            if (!isexist)
                return BadRequest(new { message = "UserName or Password already using" });

            var userdata = _userRepo.Register(mail, password);
            if (userdata == null)
            {
                return BadRequest(new { message = "Something went wrong! Try Again" });
            }
            return Ok(userdata);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [Route("registerUser")]
        public IActionResult RegisterUser(string mail, string password, string role)
        {
            bool isexist = _userRepo.IsUnique(mail);
            if (!isexist)
                return BadRequest(new { message = "UserName or Password already using" });

            var userdata = _userRepo.Register(mail, password, role);
            if (userdata == null)
            {
                return BadRequest(new { message = "Something went wrong! Try Again" });
            }
            return Ok(userdata);
        }
    }
}
