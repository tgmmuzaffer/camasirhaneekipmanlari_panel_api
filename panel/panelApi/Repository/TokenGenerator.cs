using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using panelApi.Repository.IRepository;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Repository
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly AppSettings _appSettings;

        public TokenGenerator(IOptions<AppSettings> appsettings)
        {
            _appSettings = appsettings.Value;
        }
        public string GetToken(int Id, string roleName)
        {
            string token = string.Empty;
            if (Id <= 0 || string.IsNullOrEmpty(roleName))
            {
                return token;
            }

            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaims(claims);

            var signinCredential = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokendescriptor = new SecurityTokenDescriptor();
            tokendescriptor.Subject = claimIdentity;
            tokendescriptor.Expires = DateTime.Now.AddDays(7);
            tokendescriptor.SigningCredentials = signinCredential;

            var _token = tokenhandler.CreateToken(tokendescriptor);
            token = tokenhandler.WriteToken(_token);

            return token;
        }
    }
}
