using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using www.pwa.Server.Data;
using www.pwa.Server.Filters;
using www.pwa.Server.Services;
using www.pwa.Shared;
using Microsoft.AspNetCore.Authorization;

namespace www.pwa.Server.Controllers
{
    [Authorize (Roles = "Administrator")]
    [ApiController]
    [Route("[controller]")]
    public class WwwRunAdminController : ControllerBase
    {
        private readonly ILogger<WwwRunAdminController> _logger;
        private readonly ApplicationDbContext context;
        private readonly DbService dbService;

        public WwwRunAdminController(ILogger<WwwRunAdminController> logger, ApplicationDbContext context, DbService dbService)
        {
            _logger = logger;
            this.context = context;
            this.dbService = dbService;
        }

        [HttpGet("class/{classId}")]
        public async Task<ActionResult> DeleteSchoolClass(int classId) {
            await dbService.semaphoreSlim.WaitAsync();
            try {
                var schoolClass = await context.wwwClasses.FirstOrDefaultAsync(f => f.ID == classId);
                if (schoolClass == null)
                    return NotFound();
                await dbService.Delete(context, schoolClass);
            } catch (Exception e) 
            {
                _logger.LogError($"Failed deleting schoolClass {classId}: {e.Message}");
            }
            finally {
                dbService.semaphoreSlim.Release();
            }
            return Ok();
        }

        [HttpGet("entity/{entityId}")]
        public async Task<ActionResult> DeleteEntity(int entityId) {
            await dbService.semaphoreSlim.WaitAsync();
            try {
                var entity = await context.wwwEntities.FirstOrDefaultAsync(f => f.ID == entityId);
                if (entity == null)
                    return NotFound();
                await dbService.Delete(context, entity);
            } catch (Exception e) 
            {
                _logger.LogError($"Failed deleting entity {entityId}: {e.Message}");
            }
            finally {
                dbService.semaphoreSlim.Release();
            }
            return Ok();
        }

        [HttpGet("run/{runId}")]
        public async Task<ActionResult> DeleteRun(int runId) {
            await dbService.semaphoreSlim.WaitAsync();
            try {
                var run = await context.wwwRuns.FirstOrDefaultAsync(f => f.ID == runId);
                if (run == null)
                    return NotFound();
                await dbService.Delete(context, run);
            } catch (Exception e) 
            {
                _logger.LogError($"Failed deleting run {runId}: {e.Message}");
            }
            finally {
                dbService.semaphoreSlim.Release();
            }
            return Ok();
        }
    }
}
