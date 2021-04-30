using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using www.pwa.Server.Models;

namespace www.pwa.Server.Contexts
{
    public class WwwContext : IdentityDbContext
    {
        public WwwContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<WwwWalk> wwwWalks { get; set; }
        public DbSet<WwwSchool> wwwSchools { get; set; }
        public DbSet<WwwClass> wwwClasses { get; set; }
        public DbSet<WwwEntity> wwwEntities { get; set; }
        public DbSet<WwwRun> wwwRuns { get; set; }
        public DbSet<WwwCounter> wwwCounters { get; set; }
        public DbSet<WwwCounterQueue> counterQueues { get; set; }
    }
}
