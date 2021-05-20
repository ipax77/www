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

namespace www.pwa.Server.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class WwwRunController : ControllerBase
    {
        private readonly ILogger<WwwRunController> _logger;
        private readonly ApplicationDbContext context;
        private readonly DbService dbService;

        public WwwRunController(ILogger<WwwRunController> logger, ApplicationDbContext context, DbService dbService)
        {
            _logger = logger;
            this.context = context;
            this.dbService = dbService;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Und es war Sommer";
        }

        [HttpGet]
        [Route("walk/{walkGuid}")]
        public async Task<ActionResult<WalkAppModel>> GetWalk(string walkGuid)
        {
            Guid guid;
            if (!Guid.TryParse(walkGuid, out guid))
                return NotFound();
            var walk = await context.wwwWalks
                .Include(i => i.Points)
                .Include(i => i.NextPoint)
                .Include(i => i.WwwSchools)
                .ThenInclude(j => j.WwwClasses)
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Guid == guid)
                ;
            if (walk == null)
                return NotFound();

            var walkModel = new WalkAppModel()
            {
                Name = walk.Name,
                Description = walk.Description,
                TotalDistance = walk.TotalDistance,
                CurrentDistance = Math.Round(walk.TotalRuns, 2),
                Schools = new List<School>(walk.WwwSchools.Select(t => new School()
                {
                    Name = t.Name,
                    SchoolClasses = new List<SchoolClass>(t.WwwClasses.Select(u => new SchoolClass()
                    {
                        Name = u.Name
                    }))
                })),
                Points = new List<WalkPoints>(walk.Points.OrderBy(o => o.Position).Select(s => new WalkPoints()
                {
                    Name = s.Name,
                    Description = s.Description,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Distance = s.Distance
                }))
            };
            return walkModel;
        }

        [HttpGet("gettestdata")]
        public async Task<ActionResult<RunDebugModel>> GetRunTestData()
        {
            var data = JsonSerializer.Deserialize<RunDebugModel>(await System.IO.File.ReadAllTextAsync("/data/www/runitemdata_1.json"));
            return data;
        }

        [HttpPost]
        [RequestLimit("Submit", NoOfRequest = 5, Seconds = 10)]
        public async Task<ActionResult<WwwFeedback>> Submit(EntityRunFormData data) {
            WwwFeedback feedback = await dbService.Submit(context, data);
            if (feedback == null)
                return NotFound();
            else
                return feedback;
        }

    }
}
