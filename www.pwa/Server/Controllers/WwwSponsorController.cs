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
        public async Task<ActionResult<double>> GetSponsor(string walkGuid) {
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
        public async Task CreateSponsor() {

        }
    }
}
