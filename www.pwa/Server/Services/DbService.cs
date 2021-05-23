using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using www.pwa.Server.Models;
using www.pwa.Shared;
using Microsoft.EntityFrameworkCore;
using www.pwa.Server.Data;

namespace www.pwa.Server.Services
{
    public class DbService
    {
        private readonly ILogger<DbService> logger;
        public readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public DbService(ILogger<DbService> logger)
        {
            this.logger = logger;

        }

        public async Task<float> GetCurrent(ApplicationDbContext context)
        {
            WwwWalk walk = await context.wwwWalks.FirstAsync(f => f.isActive);
            return walk.TotalRuns;
        }

        public async Task Count(ApplicationDbContext context, string name)
        {
            context.counterQueues.Add(new WwwCounterQueue()
            {
                CounterName = name,
                ValueChange = 1
            });
            await context.SaveChangesAsync();
        }

        public static async Task SetCounters(ApplicationDbContext context)
        {
            string[] names = new string[] { "GetCurrent", "Years", "Classes" };
            foreach (var name in names)
            {
                var queues = context.counterQueues.Where(x => x.CounterName == name);
                int count = await queues.CountAsync();
                var counter = context.wwwCounters.First(f => f.Name == name);
                counter.Count += count;
                context.counterQueues.RemoveRange(queues);
                await context.SaveChangesAsync();
            }
        }

        public async Task<WwwFeedback> Submit(ApplicationDbContext context, EntityRunFormData data, bool bulk = false)
        {
            Guid guid;
            if (!Guid.TryParse(data.Walk, out guid))
                return null;

            await semaphoreSlim.WaitAsync();
            WwwEntity ent = null;
            try
            {
                var walk = await context.wwwWalks
                    .Include(i => i.WwwSchools)
                    .ThenInclude(j => j.WwwClasses)
                    .FirstAsync(f => f.Guid == guid);
                if (walk == null)
                    return new WwwFeedback()
                    {
                        Error = "Lauf nicht gefunden."
                    };

                if (walk.Credential != data.Credential)
                    return new WwwFeedback()
                    {
                        Error = "Falsches Passwort."
                    };

                var school = walk.WwwSchools.FirstOrDefault(f => f.Name == data.School);
                if (school == null)
                    return new WwwFeedback()
                    {
                        Error = "Schule nicht gefunden."
                    };

                WwwClass wwwClass = school.WwwClasses.FirstOrDefault(f => f.Name == data.SchoolClass);
                if (wwwClass == null)
                    return new WwwFeedback()
                    {
                        Error = "Klasse nicht gefunden."
                    };

                WwwEntity entity = await context.wwwEntities
                    .Include(i => i.WwwClass)
                    .FirstOrDefaultAsync(f => f.Pseudonym == data.Identifier);

                if (entity == null)
                {
                    entity = new WwwEntity();
                    wwwClass.WwwSchool = school;
                    wwwClass.WwwSchool.WwwWalk = walk;
                    entity.WwwClass = wwwClass;
                    entity.Pseudonym = data.Identifier;
                    context.wwwEntities.Add(entity);

                    wwwClass.TotalEntities++;
                    school.TotalEntities++;
                    walk.TotalEntities++;
                }

                if (entity.WwwClass == wwwClass)
                {
                    WwwRun run = new WwwRun();
                    run.WwwEntity = entity;
                    run.Time = data.Time;
                    run.Distance = data.Distance;

                    context.wwwRuns.Add(run);

                    entity.TotalRuns += run.Distance;
                    wwwClass.TotalRuns += run.Distance;
                    school.TotalRuns += run.Distance;
                    walk.TotalRuns += run.Distance;

                    entity.Runs++;

                    if (bulk == false)
                    {
                        await context.SaveChangesAsync();
                        return await GetFeedback(context, walk, school, wwwClass, entity);
                    }
                    else
                    {
                        ent = entity;
                        return null;
                    }
                } else
                {
                    return new WwwFeedback()
                    {
                        Error = "Das Pseudonym ist schon in einer anderen Klasse vergeben"
                    };
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
            finally
            {
                semaphoreSlim.Release();
            }
            return new WwwFeedback() {
                Error = "Server Fehler. Bitter versuche es später noch einmal."
            };
        }

        public async Task<WwwFeedback> GetFeedback(ApplicationDbContext context, WwwWalk walk, WwwSchool school, WwwClass wwwclass, WwwEntity entity)
        {
            WwwFeedback feedback = new WwwFeedback();

            feedback.SchoolPosition = (await context.wwwSchools.AsNoTracking().OrderByDescending(o => o.TotalRuns).ToListAsync()).FindIndex(f => f.ID == school.ID) + 1;
            feedback.SchoolPercentage = MathF.Round(school.TotalRuns * 100 / walk.TotalRuns, 2);
            feedback.SchoolTotal = MathF.Round(school.TotalRuns, 2);
            feedback.ClassPosition = (await context.wwwClasses.AsNoTracking().OrderByDescending(o => o.TotalRuns).ToListAsync()).FindIndex(f => f.ID == wwwclass.ID) + 1;
            feedback.ClassPercentage = MathF.Round(wwwclass.TotalRuns * 100 / walk.TotalRuns, 2);
            feedback.ClassTotal = MathF.Round(wwwclass.TotalRuns, 2);
            feedback.EntPosition = (await context.wwwEntities.AsNoTracking().OrderByDescending(o => o.TotalRuns).ToListAsync()).FindIndex(f => f.ID == entity.ID) + 1;
            feedback.EntPercentage = MathF.Round(entity.TotalRuns * 100 / walk.TotalRuns, 2);
            feedback.EntTotal = MathF.Round(entity.TotalRuns, 2);

            var years = await context.wwwClasses.AsNoTracking().Select(s => s.Year).Distinct().ToArrayAsync();

            List<KeyValuePair<int, float>> YearList = new List<KeyValuePair<int, float>>();
            foreach (var year in years)
            {
                float sum = await context.wwwClasses.AsNoTracking().Where(x => x.Year == year).SumAsync(s => s.TotalRuns);
                YearList.Add(new KeyValuePair<int, float>(year, sum));
                if (year == wwwclass.Year)
                    feedback.YearTotal = sum;
            }
            feedback.YearTotal = MathF.Round(feedback.YearTotal, 2);
            feedback.YearPosition = YearList.Select(s => s.Value).OrderByDescending(o => o).ToList().FindIndex(f => f == feedback.YearTotal) + 1;
            feedback.YearPercentage = MathF.Round(feedback.YearTotal * 100 / walk.TotalRuns, 2);
            feedback.CurrentDistance = MathF.Round(walk.TotalRuns, 2);
            return feedback;
        }

        public async Task<List<RunInfo>> GetTableData(ApplicationDbContext context, string mode)
        {
            var walk = await context.wwwWalks.FirstAsync(f => f.isActive);
            // var school = await context.wwwSchools.FirstAsync(f => f.Name == WwwData.s_school);
            var school = await context.wwwSchools.FirstAsync(f => f.WwwWalk == walk);
            
            if (mode == "school")
            {
                
                return new List<RunInfo>()
                {
                    new RunInfo()
                    {
                        Ent = school.Name,
                        Dist = school.TotalRuns,
                        Count = school.TotalEntities
                    }
                };
            }

            if (mode == "classes")
            {
                var classes = await context.wwwClasses.Where(x => x.WwwSchool == school).ToListAsync();
                return classes.Select(s => new RunInfo()
                {
                    Ent = s.Name,
                    Dist = s.TotalRuns,
                    Count = s.TotalEntities
                }).ToList();
            }

            var wwwclass = await context.wwwClasses.FirstOrDefaultAsync(f => f.Name == mode);
            if (wwwclass != null)
            {
                var ents = await context.wwwEntities.Where(x => x.WwwClass == wwwclass).ToListAsync();
                if (ents.Any())
                    return ents.Select(s => new RunInfo()
                    {
                        Ent = s.Pseudonym,
                        Dist = s.TotalRuns,
                        Count = s.Runs
                    }).ToList();
            }

            return new List<RunInfo>();
        }

        public async Task<List<EntRunInfo>> GetEntTableData(ApplicationDbContext context, string name)
        {
            var ent = await context.wwwEntities.FirstOrDefaultAsync(f => f.Pseudonym == name);
            if (ent != null)
            {
                var runs = await context.wwwRuns.Where(x => x.WwwEntity == ent).ToListAsync();
                if (runs.Any())
                    return runs.Select(s => new EntRunInfo()
                    {
                        Date = s.Time.ToString("yyyy-MM-dd"),
                        Dist = s.Distance
                    }).ToList();
            }
            return new List<EntRunInfo>();
        }

        public async Task Delete(ApplicationDbContext context, WwwWalk walk)
        {
            var schools = await context.wwwSchools.Where(x => x.WwwWalk == walk).ToListAsync();
            foreach (var school in schools)
                await Delete(context, school, true);

            context.wwwWalks.Remove(walk);
            await context.SaveChangesAsync();
        }

        public async Task Delete(ApplicationDbContext context, WwwSchool school, bool bulk = false)
        {
            var classes = await context.wwwClasses.Where(x => x.WwwSchool == school).ToListAsync();
            foreach (var ent in classes)
                await Delete(context, ent, true);

            context.wwwSchools.Remove(school);
            if (bulk == false)
                await context.SaveChangesAsync();
        }

        public async Task Delete(ApplicationDbContext context, WwwClass wwwClass, bool bulk = false)
        {
            var entities = await context.wwwEntities.Where(x => x.WwwClass == wwwClass).ToListAsync();
            foreach (var ent in entities)
                await Delete(context, ent, true);

            context.wwwClasses.Remove(wwwClass);
            if (bulk == false)
                await context.SaveChangesAsync();
        }

        public async Task Delete(ApplicationDbContext context, WwwEntity dentity, bool bulk = false)
        {
            WwwEntity entity = await context.wwwEntities.Include(i => i.WwwClass).ThenInclude(j => j.WwwSchool).ThenInclude(k => k.WwwWalk).FirstOrDefaultAsync(f => f.ID == dentity.ID);
            var runs = await context.wwwRuns.Where(x => x.WwwEntity == entity).ToListAsync();

            foreach (var run in runs)
                await Delete(context, run, true);

            entity.WwwClass.TotalEntities--;
            entity.WwwClass.WwwSchool.TotalEntities--;
            entity.WwwClass.WwwSchool.WwwWalk.TotalEntities--;

            context.Remove(entity);
            if (bulk == false)
                await context.SaveChangesAsync();
        }

        public async Task Delete(ApplicationDbContext context, WwwRun drun, bool bulk = false)
        {
            WwwRun run = await context.wwwRuns.Include(i => i.WwwEntity).ThenInclude(j => j.WwwClass).ThenInclude(k => k.WwwSchool).ThenInclude(l => l.WwwWalk).FirstOrDefaultAsync(f => f.ID == drun.ID);
            float dist = run.Distance;

            run.WwwEntity.TotalRuns -= dist;
            run.WwwEntity.WwwClass.TotalRuns -= dist;
            run.WwwEntity.WwwClass.WwwSchool.TotalRuns -= dist;
            run.WwwEntity.WwwClass.WwwSchool.WwwWalk.TotalRuns -= dist;
            run.WwwEntity.Runs--;

            context.wwwRuns.Remove(run);
            if (bulk == false)
                await context.SaveChangesAsync();
        }

        public async Task Create(ApplicationDbContext context, WwwCreate Create)
        {
            WwwWalk walk = new WwwWalk();

            walk = new WwwWalk();
            walk.isActive = true;
            walk.Name = Create.Name;
            walk.Start = DateTime.UtcNow;
            walk.TotalDistance = Create.TotalDistance;
            context.wwwWalks.Add(walk);

            WwwSchool school = new WwwSchool();
            school.WwwWalk = walk;
            school.Name = Create.SchoolName;
            context.wwwSchools.Add(school);

            await context.SaveChangesAsync();
        }

        public async Task Seed(ApplicationDbContext context, WwwCreate NewRun = null)
        {
            Random rng = new Random();

            List<DateTime> dates = new List<DateTime>();
            DateTime edate = DateTime.Today;
            for (int i = 0; i < 60; i++)
            {
                dates.Add(edate);
                edate.AddDays(-1);
            }


            for (int i = 0; i < 1000; i++)
            {
                EntityRunFormData data = new EntityRunFormData();
                WwwWalk walk = context.wwwWalks.Include(i => i.WwwSchools).First(f => f.isActive == true);
                data.Walk = walk.Name;
                data.School = walk.WwwSchools.First().Name;
                int index = rng.Next(WwwData.s_classes.Length);
                data.SchoolClass = WwwData.s_classes[index];
                data.Distance = (float)rng.Next(1, 41);
                data.Identifier = Guid.NewGuid().ToString("N");
                index = rng.Next(dates.Count);
                data.Time = dates[index];

                var ent = await Submit(context, data, true);
            }

            await context.SaveChangesAsync();
        }

        public async Task<WwwChartInfo> GetChartData(ApplicationDbContext context, string walkGuid, string mode)
        {
            List<KeyValuePair<string, double>> data = new List<KeyValuePair<string, double>>();
            IOrderedEnumerable<KeyValuePair<string, double>> result = null;
            Guid guid;
            if (!Guid.TryParse(walkGuid, out guid))
                return null;
            var walk = await context.wwwWalks.FirstAsync(f => f.Guid == guid);
            if (walk == null)
                return null;
            var school = await context.wwwSchools.FirstAsync(f => f.WwwWalk == walk);
            if (mode == "Years")
            {
                var years = await context.wwwClasses.AsNoTracking().Select(s => s.Year).Distinct().ToListAsync();
                foreach (var year in years)
                {
                    var dists = from w in context.wwwWalks
                                from s in w.WwwSchools
                                from c in s.WwwClasses
                                from e in c.WwwEntities
                                from r in e.WwwRuns
                                where w == walk
                                where s == school
                                where c.Year == year
                                select r.Distance;
                    float sum = await dists.SumAsync(s => s);
                    if (sum > 0)
                        data.Add(new KeyValuePair<string, double>(year == 0 ? "Lehrer" : year.ToString(), Math.Round((double)sum, 2)));
                }
                result = data.OrderByDescending(o => o.Value);
            }
            else if (mode == "Classes")
            {
                var classes = await context.wwwClasses.AsNoTracking().Select(s => s.Name).Distinct().ToListAsync();
                foreach (var wwwClass in classes)
                {
                    var dists = from w in context.wwwWalks
                                from s in w.WwwSchools
                                from c in s.WwwClasses
                                from e in c.WwwEntities
                                from r in e.WwwRuns
                                where w == walk
                                where s == school
                                where c.Name == wwwClass
                                select r.Distance;
                    float sum = await dists.SumAsync(s => s);
                    if (sum > 0)
                        data.Add(new KeyValuePair<string, double>(wwwClass, Math.Round((double)sum, 2)));

                }
                result = data.OrderByDescending(o => o.Value);
            }
            else if (mode == "Schools")
            {
                var schools = await context.wwwSchools.AsNoTracking().Select(s => s.Name).Distinct().ToListAsync();
                foreach (var myschool in schools)
                {
                    var dists = from w in context.wwwWalks
                                from s in w.WwwSchools
                                from c in s.WwwClasses
                                from e in c.WwwEntities
                                from r in e.WwwRuns
                                where w == walk
                                where s.Name == myschool
                                select r.Distance;
                    float sum = await dists.SumAsync(s => s);
                    if (sum > 0)
                        data.Add(new KeyValuePair<string, double>(myschool, Math.Round((double)sum, 2)));
                }
                result = data.OrderByDescending(o => o.Value);
            } else
                return null;
            WwwChartInfo info = new WwwChartInfo()
            {
                Lables = result.Select(s => s.Key).ToList(),
                Data = result.Select(s => s.Value).ToList()
            };
            return info;
        }

        public async Task<(float, float, float)> GetDistance(ApplicationDbContext context)
        {
            var walk = await context.wwwWalks.FirstAsync(f => f.isActive);
            float total = walk.TotalDistance;
            float done = walk.TotalRuns;
            float p = MathF.Round(total * 100 / done, 2);
            if (p > 100)
                p = 100;
            return (total, done, p);
        }

        public static int GetClassYear(string name) {
            int Year = 0;
            string year = String.Empty;
            for (int i = 0; i < name.Length; i++)
            {
                if (Char.IsDigit(name[i]))
                    year += name[i];
                else
                    break;
            }
            int iyear = 0;
            if (int.TryParse(year, out iyear))
            {
                Year = iyear;
            }
            return Year;
        }
    }
}
