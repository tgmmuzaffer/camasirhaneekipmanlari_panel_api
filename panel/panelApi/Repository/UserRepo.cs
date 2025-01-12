﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using panelApi.DataAccess;
using panelApi.Models;
using panelApi.Models.Dtos;
using panelApi.RepoExtension;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<UserRepo> _logger;
        private readonly PanelApiDbcontext _panelApiDbcontext;
        public UserRepo(PanelApiDbcontext panelApiDbcontext, ITokenGenerator tokenGenerator, ILogger<UserRepo> logger)
        {
            _panelApiDbcontext = panelApiDbcontext;
            _tokenGenerator = tokenGenerator;
            _logger = logger;

        }

        public async Task<UserDto> Authenticate(string mail, string password)
        {
            try
            {
                UserDto userDto = new UserDto();
                var user = await _panelApiDbcontext.Users.FirstOrDefaultAsync(b => b.UserName == mail);
                bool isValidPass = Security.ValidateHash(password, user.Salt, user.Password);
                if (!isValidPass)
                {
                    return null;
                }

                userDto.Role = await _panelApiDbcontext.Roles.FirstOrDefaultAsync(c => c.Id == user.RoleId);
                userDto.Token = _tokenGenerator.GetToken(user.Id, userDto.Role.RoleName);
                userDto.Id = user.Id;
                userDto.Password = string.Empty;
                userDto.UserName = user.UserName;
                userDto.RoleId = userDto.Role.Id;
                return userDto;
            }
            catch (Exception e)
            {
                _logger.LogError($"UserRepo Authenticate // {e.Message}");
                return null;
            }
        }

        public async Task<bool> Delete(User entity)
        {
            try
            {
                _panelApiDbcontext.Users.Remove(entity);
                await _panelApiDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"UserRepo Delete // {e.Message}");
                return false;
            }
        }

        public async Task<User> Get(Expression<Func<User, bool>> filter = null)
        {
            try
            {
                var user = filter == null ? await _panelApiDbcontext.Users.FirstOrDefaultAsync() : await _panelApiDbcontext.Users.FirstOrDefaultAsync(filter);
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError($"UserRepo Get // {e.Message}");
                return null;
            }
        }

        public async Task<List<UserDto>> GetList(Expression<Func<User, bool>> filter = null)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError($"UserRepo GetListWithRelatedEntity // {e.Message}");
                return null;
            }
        }

        public async Task<bool> IsUnique(string mail)
        {
            try
            {
                var user = await _panelApiDbcontext.Users.FirstOrDefaultAsync(a => a.UserName == mail);
                if (user == null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"UserRepo IsUnique // {e.Message}");
                return false;
            }
        }

        public async Task<User> Register(User user)
        {
            try
            {
                var role = await _panelApiDbcontext.Roles.FirstOrDefaultAsync(c => c.Id == user.RoleId);
                _panelApiDbcontext.Users.Add(user);
                await _panelApiDbcontext.SaveChangesAsync();
                user.Password = string.Empty;
                //user.Token = _tokenGenerator.GetToken(user.Id, role.RoleName);
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError($"UserRepo Register // {e.Message}");
                return null;
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
                _logger.LogError($"UserRepo Update // {e.Message}");
                return false;
            }
        }
    }
}
