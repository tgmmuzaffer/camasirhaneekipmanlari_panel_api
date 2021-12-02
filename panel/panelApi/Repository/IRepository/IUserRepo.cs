using panelApi.Models;

namespace panelApi.Repository.IRepository
{
    public interface IUserRepo
    {
        bool IsUnique(string mail);
        User Authenticate(string mail, string password);
        User Register(string mail, string password);
    }
}
