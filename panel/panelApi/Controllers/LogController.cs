using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using panelApi.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace panelApi.Controllers
{
    [Route("api/log")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogRepo _logRepo;
        public LogController(ILogRepo logRepo)
        {
            _logRepo = logRepo;
        }

        [Authorize(Roles ="Admin, Editor")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Log))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getlogs/{count}")]
        public async Task<IActionResult> Get(int count)
        {
            var logs = await _logRepo.GetLogs(count);
            return Ok(logs);
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("deletelogs")]
        public async Task<IActionResult> DeleteLogs()
        {
            var date = DateTime.Now;
            var logs = await _logRepo.GetLogs(a=>a.TimeStamp < date);
            await _logRepo.RemoveMultiple(logs);
            return NoContent();
        }

    }
}
