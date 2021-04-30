using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using www.pwa.Server.Contexts;
using www.pwa.Server.Services;
using www.pwa.Shared;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace www.pwa.Server.Controllers
{
    [ApiController]
    public class WwwControler : ControllerBase
    {

        private readonly ILogger<WwwControler> logger;
        private readonly WwwContext context;
        private readonly DbService db;

        public WwwControler(ILogger<WwwControler> logger, DbService db, WwwContext context)
        {
            this.logger = logger;
            this.db = db;
            this.context = context;
        }

        [HttpGet("api/getcurrent")]
        public async Task<float> GetCurrent(string id)
        {
            await db.Count(context, "GetCurrent");
            return await db.GetCurrent(context);
        }

        [HttpGet("api/gettable/{mode}")]
        public async Task<List<RunInfo>> GetTableData(string mode)
        {
            return await db.GetTableData(context, mode);
        }

        [HttpGet("api/getenttable/{ent}")]
        public async Task<List<EntRunInfo>> GetEntTableData(string ent)
        {
            return await db.GetEntTableData(context, ent);
        }

        [HttpGet("api/deleteent/{ent}")]
        public async Task<IActionResult> DeleteEnt(string ent)
        {
            var myent = context.wwwEntities.FirstOrDefault(f => f.Pseudonym == ent);
            if (myent != null)
                await db.Delete(context, myent);
            else
                return NotFound();
            return Ok();
        }

        [HttpGet("api/deleterun/{ent}/{date}")]
        public async Task<IActionResult> DeleteRun(string ent, string date)
        {
            var myent = context.wwwEntities.FirstOrDefault(f => f.Pseudonym == ent);
            if (myent == null)
                return NotFound();

            var runs = await context.wwwRuns.Where(x => x.WwwEntity == myent).ToListAsync();
            runs = runs.Where(x => x.Time.ToString("yyyy-MM-dd") == date).ToList();
            if (!runs.Any())
                return NotFound();

            foreach (var run in runs)
                await db.Delete(context, run);
            
            return Ok();
        }

        [HttpGet("api/getchart/{mode}")]
        public async Task<WwwChartInfo> GetChart(string mode)
        {
            await db.Count(context, mode);
            return await db.GetChartData(context, mode);
        }

        [HttpPost("api/submit")]
        public async Task<WwwFeedback> Submit(EntityRunFormData entry) 
        {
            return await db.Submit(context, entry);
        }
    }
}
