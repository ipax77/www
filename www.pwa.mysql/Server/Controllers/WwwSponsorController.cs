using System.Diagnostics;
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
using www.pwa.Server.Models;
using System.Threading;

namespace www.pwa.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WwwSponsorController : ControllerBase
    {
        private readonly ILogger<WwwSponsorController> _logger;
        private readonly ApplicationDbContext context;
        private readonly DbService dbService;

        public WwwSponsorController(ILogger<WwwSponsorController> logger, ApplicationDbContext context, DbService dbService)
        {
            _logger = logger;
            this.context = context;
            this.dbService = dbService;
        }

        [HttpGet("{walkGuid}")]
        public async Task<ActionResult<double>> GetSponsor(string walkGuid)
        {
            Guid guid;

            if (!Guid.TryParse(walkGuid, out guid))
                return NotFound();

            var walk = await context.wwwWalks.AsNoTracking().FirstOrDefaultAsync(f => f.Guid == guid);

            if (walk == null)
                return NotFound();

            var walkSponsors = await context.walkSponsors.AsNoTracking().Where(x => x.Walk == walk).Select(s => s.CentPerKm).SumAsync();

            var entSponsors = await context.wwwEntities
                .Include(i => i.Sponsors)
                .AsNoTracking()
                .Select(s => new { Dist = s.TotalRuns, Cent = s.Sponsors.Where(x => x.Verified).Sum(t => t.CentPerKm) })
                .ToListAsync();

            double wmoney = walk.TotalRuns * walkSponsors;
            double emoney = entSponsors.Sum(s => s.Dist * s.Cent);
            return Math.Round((wmoney + emoney) / 100, 2);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSponsor(CreateSponsoresModel sponsoresModel)
        {
            return Ok();

            // Guid guid;

            // if (!Guid.TryParse(sponsoresModel.WalkGuid, out guid))
            //     return NotFound();

            // WwwWalk walk = await context.wwwWalks
            //     .Include(i => i.WwwSchools)
            //         .ThenInclude(j => j.WwwClasses)
            //         .ThenInclude(k => k.WwwEntities)
            //         .ThenInclude(l => l.Sponsors)
            //     .FirstOrDefaultAsync(f => f.Guid == guid);

            // if (walk == null)
            //     return NotFound();

            // if (sponsoresModel.Credential != walk.Credential)
            // {
            //     return Forbid();
            // }

            // WwwSchool school = walk.WwwSchools.FirstOrDefault(f => f.Name == sponsoresModel.School);
            // WwwClass schoolClass = school.WwwClasses.FirstOrDefault(f => f.Name == sponsoresModel.SchoolClass);
            // // WwwEntity entity = schoolClass.WwwEntities.FirstOrDefault(f => f.Pseudonym == sponsoresModel.EntityName);
            // WwwEntity entity = await context.wwwEntities.FirstOrDefaultAsync(f => f.Pseudonym == sponsoresModel.EntityName);

            // if (entity == null)
            // {
            //     entity = new WwwEntity()
            //     {
            //         Pseudonym = sponsoresModel.EntityName,
            //         WwwClass = schoolClass,
            //         Sponsors = new HashSet<EntitySponsor>()
            //     };
            //     context.wwwEntities.Add(entity);
            // }
            // else
            // {
            //     if (entity.WwwClass.Name != sponsoresModel.SchoolClass)
            //         return BadRequest();
            // }

            // foreach (var sponsor in sponsoresModel.Sponsors)
            // {
            //     EntitySponsor entSponsor = entity.Sponsors.FirstOrDefault(f => f.Name == sponsor.Sponsor);
            //     if (entSponsor == null)
            //     {
            //         entSponsor = new EntitySponsor()
            //         {
            //             Name = sponsor.Sponsor,
            //             CentPerKm = sponsor.CentPerKm,
            //             Verified = true,
            //             Entity = entity,
            //             Created = DateTime.UtcNow,
            //             Modified = DateTime.UtcNow
            //         };
            //         context.entitySponsors.Add(entSponsor);
            //     }
            //     else
            //     {
            //         entSponsor.CentPerKm = sponsor.CentPerKm;
            //         entSponsor.Verified = true;
            //     }
            // }

            // if (entity.hasSponsors == false)
            // {
            //     var runs = await context.wwwRuns.Where(x => x.WwwEntity == entity).SumAsync(s => s.Distance);
            //     if (runs > 0)
            //     {
            //         entity.TotalRuns += runs;
            //         schoolClass.TotalRuns += runs;
            //         school.TotalRuns += runs;
            //         walk.TotalRuns += runs;
            //     }
            //     entity.hasSponsors = true;
            // }
            // await context.SaveChangesAsync();


            // return Ok();
        }

        [HttpPost]
        [Route("entlist")]
        public async Task<ActionResult<IEnumerable<FinalSponsorResponse>>> GetSponsorListAsync(FinalSponsorRequest sponsorRequest, CancellationToken cancellationToken)
        {
            var ents = context.wwwEntities
                .AsNoTracking();

            if (!String.IsNullOrEmpty(sponsorRequest.Search))
            {
                ents = ents.Where(x => x.Pseudonym.Contains(sponsorRequest.Search));
            }

            if (!String.IsNullOrEmpty(sponsorRequest.Interest))
            {
                if (sponsorRequest.Interest == "Pseudonym")
                    if (sponsorRequest.Order)
                        ents = ents.OrderByDescending(o => o.Pseudonym);
                    else
                        ents = ents.OrderBy(o => o.Pseudonym);
                else if (sponsorRequest.Interest == "Strecke")
                    if (sponsorRequest.Order)
                        ents = ents.OrderByDescending(o => o.TotalRuns);
                    else
                        ents = ents.OrderBy(o => o.TotalRuns);
            }
            else
                ents = ents.OrderBy(o => o.Pseudonym);

            ents = ents.Skip(sponsorRequest.Skip).Take(sponsorRequest.Take);
            List<FinalSponsorResponse> lsponsors = new List<FinalSponsorResponse>();
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    lsponsors = await ents.Select(s => new FinalSponsorResponse()
                    {
                        Pseudonym = s.Pseudonym,
                        Distance = s.TotalRuns
                    })
                    .ToListAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return lsponsors;
                }
            }
            return lsponsors;
        }


        [HttpPost]
        [Route("count")]
        public async Task<int> GetSponsorCountAsync(SponsorRequest sponsorRequest)
        {
            var ents = context.wwwEntities.AsNoTracking();

            if (!String.IsNullOrEmpty(sponsorRequest.Search))
            {
                ents = ents.Where(x => x.Pseudonym.Contains(sponsorRequest.Search));
            }
            return await ents.CountAsync();
        }

        [HttpGet]
        [Route("sponsorinfo/{pseudonym}")]
        public async Task<ActionResult<List<FinalSponsorInfoResponse>>> GetSponsorInfo(string pseudonym)
        {
            var ent = await context.wwwEntities
                .Include(i => i.Sponsors)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Pseudonym == pseudonym);
            if (ent == null || !ent.Sponsors.Any())
                return NotFound();

            return ent.Sponsors.Select(s => new FinalSponsorInfoResponse()
            {
                Sponsor = s.Name,
                CentPerKm = s.CentPerKm
            }).ToList();
        }
    }
}
