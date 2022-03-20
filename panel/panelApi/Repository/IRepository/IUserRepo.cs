using panelApi.Models;
using panelApi.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IUserRepo 
    {
        Task<bool> IsUnique(string mail);
        Task<UserDto> Authenticate(string mail, string password);
        Task<User> Register(User entity);
        Task<bool> Update(User entity);
        Task<List<UserDto>> GetList(Expression<Func<User, bool>> filter = null);
        Task<User> Get(Expression<Func<User, bool>> filter = null);
        Task<bool> Delete(User entity);
    }
}
