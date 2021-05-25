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
using Microsoft.AspNetCore.Authorization;
using System.Threading;

namespace www.pwa.Server.Controllers
{
    [Authorize (Roles="Administrator")]
    [ApiController]
    [Route("[controller]")]
    public class WwwSponsorAdminController : ControllerBase
    {
        private readonly ILogger<WwwSponsorAdminController> _logger;
        private readonly ApplicationDbContext context;
        private readonly DbService dbService;

        public WwwSponsorAdminController(ILogger<WwwSponsorAdminController> logger, ApplicationDbContext context, DbService dbService)
        {
            _logger = logger;
            this.context = context;
            this.dbService = dbService;
        }

        [HttpPost]
        [Route("get")]
        public async Task<ActionResult<IEnumerable<SponsorListModel>>> GetSponsorListAsync(SponsorRequest sponsorRequest, CancellationToken cancellationToken) {

            var sponsors = context.entitySponsors
                .Include(i => i.Entity)
                    .ThenInclude(j => j.WwwClass)
                .AsNoTracking();

            sponsors = (TableBoolRequest)sponsorRequest.isVerified switch {
                TableBoolRequest.Include => sponsors.Where(x => x.Verified == true),
                TableBoolRequest.Exclude => sponsors.Where(x => x.Verified == false),
                _ => sponsors
            };

            if (!String.IsNullOrEmpty(sponsorRequest.Klasse))
                sponsors = sponsors.Where(x => x.Entity.WwwClass.Name == sponsorRequest.Klasse);

            if (!String.IsNullOrEmpty(sponsorRequest.Search)) {
                sponsors = sponsors.Where(x => x.Name.Contains(sponsorRequest.Search) 
                                            || x.Entity.Pseudonym.Contains(sponsorRequest.Search));
            }

            if (!String.IsNullOrEmpty(sponsorRequest.Interest)) {
                if (typeof(EntitySponsor).GetProperty(sponsorRequest.Interest) != null)
                    sponsors = Extensions.CallOrderedQueryable(sponsors, sponsorRequest.Order ? "OrderBy" : "OrderByDescending", sponsorRequest.Interest);
                else {
                    sponsors = (sponsorRequest.Interest, sponsorRequest.Order) switch {
                        ("Klasse", true) => sponsors.OrderBy(o => o.Entity.WwwClass.Name),
                        ("Klasse", false) => sponsors.OrderByDescending(o => o.Entity.WwwClass.Name),
                        ("Pseudonym", true) => sponsors.OrderBy(o => o.Entity.Pseudonym),
                        ("Pseudonym", false) => sponsors.OrderByDescending(o => o.Entity.Pseudonym),
                        ("Sponsor", true) => sponsors.OrderBy(o => o.Name),
                        ("Sponsor", false) => sponsors.OrderByDescending(o => o.Name),                        
                        _ => sponsors.OrderBy(o => o.ID)
                    };
                }
            } else
                sponsors = sponsors.OrderBy(o => o.ID);

            sponsors = sponsors.Skip(sponsorRequest.Skip).Take(sponsorRequest.Take);
            List<SponsorListModel> lsponsors = new List<SponsorListModel>();
            if (!cancellationToken.IsCancellationRequested) {
                try {
                    lsponsors = await sponsors.Select(s => new SponsorListModel() {
                        ID = s.ID,
                        Pseudonym = s.Entity.Pseudonym,
                        Klasse = s.Entity.WwwClass.Name,
                        Sponsor = s.Name,
                        CentPerKm = s.CentPerKm,
                        Verified = s.Verified,
                        Created = s.Created,
                        Modified = s.Modified
                    })
                    .ToListAsync(cancellationToken);
                } catch (OperationCanceledException) {
                    return lsponsors;
                }
            }
            return lsponsors;
        }
    

        [HttpPost]
        [Route("count")]
        public async Task<int> GetSponsorCountAsync(SponsorRequest sponsorRequest) {

            var sponsors = context.entitySponsors
                .Include(i => i.Entity)
                    .ThenInclude(j => j.WwwClass)
                .AsNoTracking();

            sponsors = (TableBoolRequest)sponsorRequest.isVerified switch {
                TableBoolRequest.Include => sponsors.Where(x => x.Verified == true),
                TableBoolRequest.Exclude => sponsors.Where(x => x.Verified == false),
                _ => sponsors
            };

            if (!String.IsNullOrEmpty(sponsorRequest.Klasse))
                sponsors = sponsors.Where(x => x.Entity.WwwClass.Name == sponsorRequest.Klasse);

            if (!String.IsNullOrEmpty(sponsorRequest.Search)) {
                sponsors = sponsors.Where(x => x.Name.Contains(sponsorRequest.Search) 
                                            || x.Entity.Pseudonym.Contains(sponsorRequest.Search));
            }

            return await sponsors.CountAsync();
        }

        [HttpGet]
        [Route("verify/{id}")]
        public async Task<ActionResult> Verify(int id) {
            var sponsor = await context.entitySponsors.FirstOrDefaultAsync(f => f.ID == id);
            if (sponsor == null)
                return NotFound();

            sponsor.Verified = true;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("unverify/{id}")]
        public async Task<ActionResult> UnVerify(int id) {
            var sponsor = await context.entitySponsors.FirstOrDefaultAsync(f => f.ID == id);
            if (sponsor == null)
                return NotFound();

            sponsor.Verified = false;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(int id) {
            var sponsor = await context.entitySponsors.FirstOrDefaultAsync(f => f.ID == id);
            if (sponsor == null)
                return NotFound();

            context.entitySponsors.Remove(sponsor);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
