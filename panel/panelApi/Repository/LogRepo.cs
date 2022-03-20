using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using panelApi.DataAccess;
using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository
{
    public class LogRepo : ILogRepo
    {
        private readonly ILogger<LogRepo> _logger;
        private readonly LogDbcontext _logDbcontext;
        public LogRepo(LogDbcontext logDbcontext, ILogger<LogRepo> logger)
        {
            _logger = logger;
            _logDbcontext = logDbcontext;
        }

        public async Task<List<Log>> GetLogs(Expression<Func<Log, bool>> filter)
        {
            try
            {
                return filter == null ?
                               await _logDbcontext.Logs
                               .OrderBy(b => b.TimeStamp)
                               .ToListAsync() :
                               await _logDbcontext.Logs
                               .Where(filter)
                               .OrderBy(b => b.TimeStamp)
                               .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"LogRepo GetLogs with parameter // {e.Message}");
                return null;
            }

        }

        public async Task<List<Log>> GetLogs(int count)
        {
            try
            {
                return await _logDbcontext.Logs.Take(count).OrderBy(a => a.TimeStamp).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"LogRepo GetLogs // {e.Message}");
                return null;
            }

        }

        public async Task RemoveMultiple(ICollection<Log> logs)
        {
            using var transaction = _logDbcontext.Database.BeginTransaction();
            try
            {
                _logDbcontext.RemoveRange(logs);
                await _logDbcontext.SaveChangesAsync();

                transaction.Commit();

            }
            catch (Exception e)
            {
                _logger.LogError($"LogRepo RemoveMultiple // {e.Message}");
                transaction.Rollback();
            }
        }
    }
}
