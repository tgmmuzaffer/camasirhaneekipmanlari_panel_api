using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface ITokenGenerator
    {
        string GetToken(int Id, string roleName);
    }
}
