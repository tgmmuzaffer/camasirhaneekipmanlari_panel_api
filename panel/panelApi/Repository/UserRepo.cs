using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using panelApi.DataAccess;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace panelApi.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly PanelApiDbcontext _panelApiDbcontext;
        private readonly AppSettings _appSettings;
        public UserRepo(PanelApiDbcontext panelApiDbcontext, IOptions<AppSettings> appsettings)
        {
            _panelApiDbcontext = panelApiDbcontext;
            _appSettings = appsettings.Value;
        }
        public User Authenticate(string mail, string password)
        {
            var user = _panelApiDbcontext.Users.FirstOrDefault(x => x.UserName == mail && x.Password == password);
            if (user == null)
            {
                return null;
            }
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = System.DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokendescriptor);
            user.Token = tokenhandler.WriteToken(token);
            user.Password = string.Empty;
            return user;
        }

        public bool IsUnique(string mail)
        {
            var user = _panelApiDbcontext.Users.FirstOrDefault(a => a.UserName == mail);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public User Register(string mail, string password, string role=null)
        {
            User user = new User
            {
                UserName = mail,
                Password = password,
                Role=role
            };
            _panelApiDbcontext.Users.Add(user);
            _panelApiDbcontext.SaveChanges();
            user.Password = string.Empty;
            return user;
        }
    }
}
