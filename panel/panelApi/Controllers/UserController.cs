using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.RepoExtension;
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
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepo userRepo, IRoleRepo roleRepo, ILogger<UserController> logger)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {

            var userdata = await _userRepo.Authenticate(user.UserName, user.Password);
            if (user == null)
            {
                _logger.LogWarning($"{user.UserName} adlı kullanıcı giriş yapmaya çalıştı.");
                return BadRequest(new { message = "UserName or Password is incorrect" });
            }

            _logger.LogWarning($"{user.UserName} adlı kullanıcı giriş yaptı.");
            return Ok(userdata);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            bool isexist = await _userRepo.IsUnique(user.UserName);
            if (!isexist)
            {
                _logger.LogError("Register__Kullanıcı zaten mevcut");
                return BadRequest(new { message = "UserName or Password already using" });
            }

            var secdata = Security.Get(user.Password);
            if (secdata == null)
            {
                _logger.LogError($"Register/Fail__{user.UserName} isimli Kullanıcının hash i oluşturulurken hata meydana geldi.");
                return BadRequest(new { message = "User could not created." });
            }

            user.Password = secdata[0];
            user.Salt = secdata[2];
            var userdata = await _userRepo.Register(user);
            if (userdata == null)
            {
                _logger.LogError($"Register/Fail__{user.UserName} isimli Kullanıcı oluşturulurken hata meydana geldi.");
                return BadRequest(new { message = "Something went wrong! Try Again" });
            }

            _logger.LogWarning($"Register/Success__{user.UserName} isimli Kullanıcı oluşturuldu.");
            return Ok(userdata);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("registerUser")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            var isexist = await _userRepo.Get(a => a.UserName == user.UserName);
            if (isexist != null)
            {
                _logger.LogError("RegisterUser__Kullanıcı bulunamadı");
                return BadRequest(new { message = "UserName not found" });
            }

            var secdata = Security.Get(user.Password);
            user.Password = secdata[0];
            user.Salt = secdata[2];
            var userdata = await _userRepo.Register(user);
            if (userdata == null)
            {
                _logger.LogError($"RegisterUser/Fail__{user.UserName} isimli Kullanıcının hash i oluşturulurken hata meydana geldi.");
                return BadRequest(new { message = "Something went wrong! Try Again" });
            }

            _logger.LogWarning($"RegisterUser/Success__{user.UserName} isimli Kullanıcı oluşturuldu.");
            return Ok(userdata.Id);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getUser/{Id}")]
        public async Task<IActionResult> GetUser(int Id)
        {
            var user = await _userRepo.Get(a => a.Id == Id);
            if (user == null)
            {
                _logger.LogError($"GetUser/Fail__{Id} Id'li Kullanıcı bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            var role = await _roleRepo.GetRole(r => r.Id == user.RoleId);
            if (role == null)
            {
                _logger.LogError($"GetUser/Fail__{user.UserName} Isimli Kullanıcı Rolü bulunamdı.");
                ModelState.AddModelError("", "User Role not found");
                return StatusCode(404, ModelState);
            }

            UserDto userDto = new UserDto
            { Id = user.Id, Password = user.Password, RoleId = role.Id, UserName = user.UserName };

            return Ok(userDto);
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getMyData/{username}")]
        public async Task<IActionResult> GetMyData(string username)
        {
            var user = await _userRepo.Get(a => a.UserName == username);
            if (user == null)
            {
                _logger.LogError($"GetMyData/Fail__{username} İsimli Kullanıcı bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            return Ok(user);
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateMyDataContent")]
        public async Task<IActionResult> UpdateMyDataContent(UserDto userDto)
        {
            var userdata = await _userRepo.Get(a => a.Id == userDto.Id);
            User user = new User();
            if (userdata.Id <= 0)
            {
                _logger.LogError($"UpdateMyDataContent/Fail__{userDto.UserName} İsimli Kullanıcı bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }
            if (userDto.NewPassword != null)
            {
                bool isValid = Security.ValidateHash(userDto.Password, userdata.Salt, userdata.Password);
                if (isValid)
                {
                    var securtyData = Security.Get(userDto.NewPassword);
                    user.Id = userdata.Id;
                    user.Password = securtyData[0];
                    user.ResetPassword = userdata.ResetPassword;
                    user.RoleId = userdata.RoleId;
                    user.Salt = securtyData[2];
                    user.UserName = userDto.UserName;
                    var result = await _userRepo.Update(user);
                    if (!result)
                    {
                        if (userdata.UserName != user.UserName)
                        {
                            _logger.LogError($"UpdateMyDataContent/Fail__{userdata.UserName} isimli Kullanıcı Adı {userDto.UserName} olarak değiştirilemdi.");
                        }

                        _logger.LogError($"UpdateMyDataContent/Fail__{userdata.UserName} isimli Kullanıcının Şifresi güncellenemedi.");
                        ModelState.AddModelError("", "User could not updated");
                        return StatusCode(500, ModelState);
                    }

                    _logger.LogWarning($"UpdateMyDataContent/Success__{userDto.UserName} isimli Kullanıcının bilgileri güncellendi.");
                    return NoContent();
                }
                else
                {
                    _logger.LogError($"UpdateMyDataContent/Fail__{userdata.UserName} isimli Kullanıcının eski şifresi eşleşmiyor.");
                    ModelState.AddModelError("", "Password not match");
                    return StatusCode(400, ModelState);
                }
            }
            else
            {
                user.Id = userdata.Id;
                user.Password = userdata.Password;
                user.ResetPassword = userdata.ResetPassword;
                user.RoleId = userdata.RoleId;
                user.Salt = userdata.Salt;
                user.UserName = userDto.UserName;
                var result = await _userRepo.Update(user);
                if (!result)
                {
                    _logger.LogError($"UpdateMyDataContent/Fail__{userdata.UserName} isimli Kullanıcının bilgileri güncellenemedi.");
                    ModelState.AddModelError("", "User could not updated");
                    return StatusCode(500, ModelState);
                }

                _logger.LogWarning($"UpdateMyDataContent/Success__{userDto.UserName} isimli Kullanıcının bilgileri güncellendi.");
                return NoContent();
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getByUserName/{username}")]
        public async Task<IActionResult> GetByUserName(string username)
        {
            var userData = await _userRepo.Get(a => a.UserName == username);
            if (userData == null)
            {
                _logger.LogError("GetByUserName/Fail__{username} Isimli Kullanıcı bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            var role = await _roleRepo.GetRole(r => r.Id == userData.RoleId);
            if (role == null)
            {
                _logger.LogError($"GetByUserName/Fail__{username} Isimli Kullanıcı rolü bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            UserDto userDto = new UserDto
            { Id = userData.Id, Password = userData.Password, RoleId = role.Id, UserName = userData.UserName, Salt = userData.Salt };

            return Ok(userDto);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getByResetPass/{resetPass}")]
        public async Task<IActionResult> GetByResetPass(string resetPass)
        {
            var userData = await _userRepo.Get(a => a.ResetPassword == resetPass);
            if (userData == null)
            {
                _logger.LogError($"GetByResetPass/Fail__{resetPass} koda sahip Kullanıcı bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }
            var role = await _roleRepo.GetRole(r => r.Id == userData.RoleId);
            if (role == null)
            {
                _logger.LogError($"GetByResetPass/Fail__{userData.UserName} isismli Kullanıcı bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            UserDto userDto = new UserDto
            { Id = userData.Id, Password = userData.Password, Salt = userData.Salt, RoleId = role.Id, UserName = userData.UserName, ResetPassword = userData.ResetPassword };
            return Ok(userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var user = await _userRepo.GetList();
            if (user.Count < 0)
            {
                _logger.LogError("GetAllUser/Fail__Kullanıcılar bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var userdata = await _userRepo.Get(a => a.Id == user.Id);
            if (userdata.Id <= 0)
            {
                _logger.LogError($"UpdateReferance__{user.UserName} isimli_{user.Id} Id'li kullanıcı bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }
            user.Id = userdata.Id;
            user.Password = userdata.Password;
            user.ResetPassword = userdata.ResetPassword;
            user.Salt = userdata.Salt;

            var result = await _userRepo.Update(user);
            if (!result)
            {
                _logger.LogError($"UpdateUser/Fail__{user.UserName} isimli Kullanıcı güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "User could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateUser/Success__{user.UserName} isimli_{user.Id} id'li Kullanıcı güncellendi");
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("updateResetUser")]
        public async Task<IActionResult> UpdateResetUser([FromBody] User user)
        {
            var secdata = Security.Get(user.Password);
            if (secdata == null)
            {
                user.Password = null;
                user.Salt = null;
            }
            else
            {
                user.Password = secdata[0];
                user.Salt = secdata[2];
            }

            var result = await _userRepo.Update(user);
            if (!result)
            {
                _logger.LogError($"UpdateResetUser/Fail__{user.UserName} isimli Kullanıcı güncellenirken hata meydana geldi.");
                ModelState.AddModelError("", "User could not updated");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"UpdateResetUser/Success__{user.UserName} isimli_{user.Id} id'li Kullanıcı güncellendi");
            return NoContent();
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("deleteUser/{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var user = await _userRepo.Get(a => a.Id == Id);
            if (user == null)
            {
                _logger.LogError($"DeleteUser__{Id} Id'li Kullanıcı bulunamdı.");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            var result = await _userRepo.Delete(user);
            if (!result)
            {
                _logger.LogError($"DeleteUser/Fail__{user.UserName} isimli Kullanıcı silinirken hata oluştu.");
                ModelState.AddModelError("", "user could not deleted");
                return StatusCode(500, ModelState);
            }

            _logger.LogWarning($"DeleteUser/Success__{user.UserName} isimli Kullanıcı silindi.");
            return NoContent();
        }
    }
}
