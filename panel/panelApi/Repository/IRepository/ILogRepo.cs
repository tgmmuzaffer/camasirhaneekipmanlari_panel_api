using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface ILogRepo 
    {
        Task RemoveMultiple(ICollection<Log> logs);
        Task<ICollection<Log>> GetLogs(Expression<Func<Log, bool>> filter = null);
        Task<ICollection<Log>> GetLogs(int count);

    }
}
