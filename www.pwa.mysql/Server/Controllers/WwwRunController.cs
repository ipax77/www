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
                .SingleOrDefaultAsync(s => s.Guid == guid);
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
                    SchoolClasses = Extensions.SortClasses(new List<SchoolClass>(t.WwwClasses.Select(u => new SchoolClass()
                    {
                        Name = u.Name
                    })))
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

        [HttpGet("tabledata/schools/{walkguid}")]
        public async Task<ActionResult<List<SchoolInfoModel>>> GetWalkSchoolsData(string walkguid)
        {
            Guid guid;
            if (!Guid.TryParse(walkguid, out guid))
                return NotFound();
            var walk = await context.wwwWalks
                .Include(i => i.WwwSchools)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Guid == guid);

            if (walk == null || !walk.WwwSchools.Any())
                return NotFound();
            return new List<SchoolInfoModel>(walk.WwwSchools.Select(s => new SchoolInfoModel()
            {
                Id = s.ID,
                Name = s.Name,
                Distance = MathF.Round(s.TotalRuns, 2)
            }));
        }

        [HttpGet("tabledata/classes/{walkguid}/{schoolId}")]
        public async Task<ActionResult<List<SchoolClassInfoModel>>> GetSchoolClassesData(string walkguid, int schoolId)
        {
            Guid guid;

            if (!Guid.TryParse(walkguid, out guid))
                return NotFound();

            var school = await context.wwwSchools
                .Include(i => i.WwwWalk)
                .Include(j => j.WwwClasses)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.WwwWalk.Guid == guid && f.ID == schoolId);

            if (school == null || !school.WwwClasses.Any())
                return NotFound();

            return new List<SchoolClassInfoModel>(school.WwwClasses.Select(s => new SchoolClassInfoModel()
            {
                Id = s.ID,
                Name = s.Name,
                Distance = MathF.Round(s.TotalRuns, 2)
            }));
        }

        [HttpGet("tabledata/entities/{schoolId}/{classId}")]
        public async Task<ActionResult<List<EntityInfoModel>>> GetClassEntitiesData(int schoolId, int classId)
        {

            var schoolclass = await context.wwwClasses
                .Include(i => i.WwwSchool)
                .Include(j => j.WwwEntities)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.WwwSchool.ID == schoolId && f.ID == classId);

            if (schoolclass == null || !schoolclass.WwwEntities.Any())
                return NotFound();

            return new List<EntityInfoModel>(schoolclass.WwwEntities.Select(s => new EntityInfoModel()
            {
                Id = s.ID,
                Name = s.Pseudonym,
                Distance = MathF.Round(s.TotalRuns, 2)
            }));
        }

        [HttpGet("tabledata/runs/{classId}/{entityId}")]
        public async Task<ActionResult<List<EntityRunInfoModel>>> GetEntityRunsData(int classId, int entityId)
        {

            var entity = await context.wwwEntities
                .Include(i => i.WwwClass)
                .Include(j => j.WwwRuns)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.WwwClass.ID == classId && f.ID == entityId);

            if (entity == null || !entity.WwwRuns.Any())
                return NotFound();

            return new List<EntityRunInfoModel>(entity.WwwRuns.Select(s => new EntityRunInfoModel()
            {
                Id = s.ID,
                Time = s.Time,
                Distance = MathF.Round(s.Distance, 2)
            }));
        }

        [HttpGet("gettestdata")]
        public async Task<ActionResult<RunDebugModel>> GetRunTestData()
        {
            var data = JsonSerializer.Deserialize<RunDebugModel>(await System.IO.File.ReadAllTextAsync("/data/www/runitemdata_1.json"));
            return data;
        }

        [HttpGet("chart/{guid}/{mode}")]
        public async Task<ActionResult<WwwChartInfo>> GetChartData(string guid, string mode)
        {
            var info = await dbService.GetChartData(context, guid, mode);
            if (info == null)
                return NotFound();
            else
                return info;
        }

        [HttpPost]
        [RequestLimit("Submit", NoOfRequest = 5, Seconds = 10)]
        public async Task<ActionResult<WwwFeedback>> Submit(EntityRunFormData data)
        {
            WwwFeedback feedback = await dbService.Submit(context, data);
            if (feedback == null)
                return NotFound();
            else
                return feedback;
        }

        [HttpGet("info/{point}")]
        public async Task<ActionResult<WalkPoints>> GetPointInfo(string point)
        {
            var dbpoint = await context.wwwWalkDatas.FirstOrDefaultAsync(f => f.Name == point);
            if (dbpoint == null)
            {
                return NotFound();
            }

            return new WalkPoints()
            {
                Name = dbpoint.Name,
                Description = dbpoint.Description,
                Latitude = dbpoint.Latitude,
                Longitude = dbpoint.Longitude,
                Distance = dbpoint.Distance,
                LongDescription = dbpoint.LongDescription,
                ImageCopyRight = dbpoint.ImageCopyRight,
                ImageCopyRightLink = dbpoint.ImageCopyRightLink
            };
        }
    }
}
