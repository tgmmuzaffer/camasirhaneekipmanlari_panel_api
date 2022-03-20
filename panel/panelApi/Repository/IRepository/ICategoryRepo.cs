using panelApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface ICategoryRepo : IBaseRepo<Category>
    {
        Task<List<Category>> GetNameList();
        Task<Category> GetName(int Id);

    }
}
