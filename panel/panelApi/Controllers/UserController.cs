using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.Repository.IRepository;
using System.Threading.Tasks;

namespace panelApi.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IRoleRepo _roleRepo;
        public UserController(IUserRepo userRepo, IRoleRepo roleRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            var userdata =await _userRepo.Authenticate(user.UserName, user.Password);
            if (userdata == null)
            {
                return BadRequest(new { message = "UserName or Password is incorrect" });
            }
            return Ok(userdata);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            bool isexist =await _userRepo.IsUnique(user.UserName);
            if (!isexist)
                return BadRequest(new { message = "UserName or Password already using" });

            var userdata = _userRepo.Register(user);
            if (userdata == null)
            {
                return BadRequest(new { message = "Something went wrong! Try Again" });
            }
            return Ok(userdata);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [Route("registerUser")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            bool isexist =await _userRepo.IsUnique(user.UserName);
            if (!isexist)
                return BadRequest(new { message = "UserName or Password already using" });
            
            var userdata = _userRepo.Register(user);
            if (userdata == null)
            {
                return BadRequest(new { message = "Something went wrong! Try Again" });
            }
            return Ok(userdata);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getUser/{Id}")]
        public async Task<IActionResult> GetUser(int Id)
        {
            var user = await _userRepo.Get(a => a.Id == Id);
            var role= await _roleRepo.GetRole(r => r.Id == user.RoleId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }
            UserDto userDto = new UserDto
            { Id = user.Id, Password = user.Password, RoleId = role.Id, UserName = user.UserName };

            return Ok(userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getByUserName")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            var user = await _userRepo.Get(a => a.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var user = await _userRepo.GetList();
            if (user.Count<0)
            {
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }
            return Ok(user);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var userdata = await _userRepo.Get(a => a.Id == user.Id);
            if (userdata.Id<=0)
            {
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }
            userdata = user;
            var result = await _userRepo.Update(user);
            if (!result)
            {
                ModelState.AddModelError("", "User could not updated");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteUser/{Id}")]
        public async Task<IActionResult> DeleteProperty(int Id)
        {
            var user = await _userRepo.Get(a => a.Id == Id);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            var result = await _userRepo.Delete(user);
            if (!result)
            {
                ModelState.AddModelError("", "user could not deleted");
                return StatusCode(500, ModelState);
            }
            return Ok();
        }
    }
}
