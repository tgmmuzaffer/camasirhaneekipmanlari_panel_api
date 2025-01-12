﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace panelApi.Repository
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly ILogger<TokenGenerator> _logger;
        private readonly AppSettings _appSettings;

        public TokenGenerator(IOptions<AppSettings> appsettings, ILogger<TokenGenerator> logger)
        {
            _logger = logger;
            _appSettings = appsettings.Value;
        }
        public string GetToken(int Id, string roleName)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError($"TokenGenerator GetToken // {e.Message}");
                return null;
            }

        }
    }
}
