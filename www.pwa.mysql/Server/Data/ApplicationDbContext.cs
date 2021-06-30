using www.pwa.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace www.pwa.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<WwwWalk> wwwWalks { get; set; }
        public DbSet<WwwSchool> wwwSchools { get; set; }
        public DbSet<WwwClass> wwwClasses { get; set; }
        public DbSet<WwwEntity> wwwEntities { get; set; }
        public DbSet<WwwRun> wwwRuns { get; set; }
        public DbSet<WwwCounter> wwwCounters { get; set; }
        public DbSet<WwwCounterQueue> counterQueues { get; set; }
        public DbSet<EntitySponsor> entitySponsors { get; set; }
        public DbSet<WalkSponsor> walkSponsors { get; set; }
        public DbSet<WwwWalkData> wwwWalkDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WwwWalk>(entitiy => {
                entitiy.HasIndex(i => i.Guid)
                    .IsUnique();
            });
        }
    }
}
