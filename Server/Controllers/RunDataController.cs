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
using System.Text.Json;

namespace www.pwa.Server.Controllers
{
    [ApiController]
    public class RunDataControler : ControllerBase
    {

        private readonly ILogger<RunDataControler> logger;
        private readonly WwwContext context;
        private readonly DbService db;

        public RunDataControler(ILogger<RunDataControler> logger, DbService db, WwwContext context)
        {
            this.logger = logger;
            this.db = db;
            this.context = context;
        }

        [HttpPost("api/rundata")]
        public async Task<ActionResult> GetRunData(EntityRunFormData rundata)
        {
            
            return Ok();
        }

        [HttpPost("api/testdata")]
        public async Task<ActionResult> GetData(List<double[]> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync("/data/rundata.json", json);
            return Ok();
        }

        [HttpGet("api/testrun")]
        public async Task<ActionResult<List<double[]>>> GetTestData() {
            var data = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<double[]>>(System.IO.File.ReadAllText("/data/rundata.json"));
            Random rng = new Random();
            double mod1 = (rng.NextDouble() + rng.Next(1, 4)) * rng.Next(1, 3);
            double mod2 = (rng.NextDouble() + rng.Next(1, 4)) * rng.Next(1, 3);
            if (rng.Next(0, 2) == 0)
                data.ForEach(f => f[0] += mod1 );
            else
                data.ForEach(f => f[0] -= mod1 );
            if (rng.Next(0, 2) == 0)
                data.ForEach(f => f[1] += mod2 );
            else
                data.ForEach(f => f[1] -= mod2 );
            return data;
        }
    }
}
