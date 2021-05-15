using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using www.pwa.Shared;

namespace www.pwa.Client.Services
{
    public class DataService
    {
        private readonly HttpClient Http;
        private readonly ILogger<DataService> logger;
        public bool isAdmin { get; private set; } = false;

        public DataService(HttpClient http, ILogger<DataService> logger)
        {
            Http = http;
            this.logger = logger;
        }

        public async Task<WwwFeedback> Submit(EntityRunFormData formData)
        {
            WwwFeedback feedback = null;
            HttpResponseMessage response = null;
            if (isAdmin)
                isAdmin = false;
            try
            {
                response = await Http.PostAsJsonAsync<EntityRunFormData>("api/submit", formData);
            } catch (Exception e)
            {
                logger.LogError(e.Message);
                return feedback;
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(content))
                        feedback = JsonSerializer.Deserialize<WwwFeedback>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }
                if (feedback.EntPosition == 1337 && feedback.SchoolTotal == 1337)
                {
                    isAdmin = true;
                    return null;
                }
            }
            return feedback;
        }

        public async Task<float> GetCurrentDistance()
        {
            float dist = 0;
            HttpResponseMessage response;
            try
            {
                response = await Http.GetAsync("api/getcurrent");
            } catch (Exception e)
            {
                logger.LogError(e.Message);
                return dist;
            }
            float.TryParse(await response.Content.ReadAsStringAsync(), out dist);
            return dist;
        }

        public async Task<List<RunInfo>> GetTableData(string mode)
        {
            try
            {
                return await Http.GetFromJsonAsync<List<RunInfo>>($"api/gettable/{mode}");
            } catch (Exception e)
            {
                logger.LogError(e.Message);
            }
            return new List<RunInfo>();
        }

        public async Task<List<EntRunInfo>> GetEntTableData(string ent)
        {
            try
            {
                return await Http.GetFromJsonAsync<List<EntRunInfo>>($"api/getenttable/{ent}");
            } catch (Exception e)
            {
                logger.LogError(e.Message);
            }
            return new List<EntRunInfo>();
        }

        public async Task DeleteEnt(string ent)
        {
            try
            {
                await Http.GetAsync("api/deleteent/" + ent);
            } catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public async Task DeleteEntRun(string ent, string run)
        {
            try
            {
                await Http.GetAsync($"api/deleterun/{ent}/{run}");
            } catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public async Task<WwwChartInfo> GetChartData(string mode)
        {
            try
            {
                return await Http.GetFromJsonAsync<WwwChartInfo>($"api/getchart/" + mode);
            } catch (Exception e)
            {
                logger.LogError(e.Message);
            }
            return null;
        }

        public async Task SetPath(string jsonfile = "")
        {
            if (!WwwData.pathPoints.Any())
            {
                try
                {
                    if (String.IsNullOrEmpty(jsonfile))
                        WwwData.pathPoints = await Http.GetFromJsonAsync<List<WwwPathPoint>>("json/earth1.json");
                    else
                        WwwData.pathPoints = await Http.GetFromJsonAsync<List<WwwPathPoint>>("json/" + jsonfile);
                } catch (Exception e)
                {
                    logger.LogError(e.Message);
                }

            }
        }

        // DEBUG
        public async Task SendDebugRun(RunDebugModel data) {
            await Http.PostAsJsonAsync("api/testdata", data);
        }

        public async Task SendDebugRunItem(RunDebugItemModel data) {
            await Http.PostAsJsonAsync("api/testrunitem", data);
        }
    }
}
