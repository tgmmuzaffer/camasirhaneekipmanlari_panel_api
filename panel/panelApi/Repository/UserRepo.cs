using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using panelApi.DataAccess;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<UserDto> Authenticate(string mail, string password)
        {
            UserDto userDto = new UserDto();

            var user = await _panelApiDbcontext.Users.FirstOrDefaultAsync(b => b.UserName == mail && b.Password == password);
            userDto.Role = await _panelApiDbcontext.Roles.FirstOrDefaultAsync(c => c.Id == user.RoleId);
            if (user == null)
            {
                return null;
            }
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, userDto.Role.RoleName));

            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaims(claims);

            var signinCredential = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokendescriptor = new SecurityTokenDescriptor();
            tokendescriptor.Subject = claimIdentity;
            tokendescriptor.Expires = DateTime.Now.AddDays(7);
            tokendescriptor.SigningCredentials = signinCredential;

            var token = tokenhandler.CreateToken(tokendescriptor);
            userDto.Token = tokenhandler.WriteToken(token);
            userDto.Id = user.Id;
            userDto.Password = string.Empty;
            userDto.UserName = user.UserName;
            userDto.RoleId = userDto.Role.Id;
            return userDto;
        }


        public async Task<bool> Delete(User entity)
        {
            _panelApiDbcontext.Users.Remove(entity);
            await _panelApiDbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<User> Get(Expression<Func<User, bool>> filter = null)
        {
            var user = filter == null ? await _panelApiDbcontext.Users.FirstOrDefaultAsync() : await _panelApiDbcontext.Users.FirstOrDefaultAsync(filter);
            return user;
        }

        public async Task<ICollection<UserDto>> GetList(Expression<Func<User, bool>> filter = null)
        {
            List<UserDto> userlistDto = new List<UserDto>();
            var user = filter == null ? await _panelApiDbcontext.Users.ToListAsync() : await _panelApiDbcontext.Users.Where(filter).ToListAsync();
            foreach (var item in user)
            {
                UserDto userDto = new UserDto();
                userDto.Id = item.Id;
                userDto.Password = item.Password;
                userDto.Role = await _panelApiDbcontext.Roles.FirstOrDefaultAsync(a => a.Id == item.RoleId);
                userDto.Token = item.Token;
                userDto.UserName = item.UserName;
                userDto.RoleId = item.RoleId;
                userlistDto.Add(userDto);
            }
            return userlistDto;
        }

        public async Task<bool> IsUnique(string mail)
        {
            var user = await _panelApiDbcontext.Users.FirstOrDefaultAsync(a => a.UserName == mail);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<User> Register(User user)
        {
            try
            {
                _panelApiDbcontext.Users.Add(user);
                await _panelApiDbcontext.SaveChangesAsync();
                user.Password = string.Empty;
                return user;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task<bool> Update(User entity)
        {
            try
            {
                _panelApiDbcontext.Users.Update(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }
    }
}
