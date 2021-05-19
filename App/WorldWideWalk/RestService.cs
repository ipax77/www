using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorldWideWalk.Models;

namespace WorldWideWalk
{
    public class RestService : IRestService
    {
        HttpClient client;

        public RestService()
        {
            client = new HttpClient();
        }

        public async Task<List<WalkAppModel>> GetWalks()
        {
            Uri uri = new Uri(string.Format(App.API, "getwalks"));

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<WalkAppModel>>(content);
                }
            } catch
            {

            }
            return new List<WalkAppModel>()
            {
                new WalkAppModel()
                {
                    Name = "WorldWideWalk"
                }
            };
        }

        public async Task<WalkAppModel> GetWalk(string guid = "7A40C465-BDC8-4373-B6BE-6E49C10D5ECA")
        {
            string uri = App.API + $"walk/{guid}";
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WalkAppModel>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public async Task<WwwFeedback> SubmitRun(EntityRunFormData data)
        {
            Uri uri = new Uri(string.Format(App.API, "submit"));
            string json = JsonSerializer.Serialize(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            WwwFeedback feedback = null;
            try
            {
                response = await client.PostAsync(uri, content);
                var resContent = await response.Content.ReadAsStringAsync();
                feedback = JsonSerializer.Deserialize<WwwFeedback>(resContent);
            }
            catch (Exception e)
            {
                return new WwwFeedback()
                {
                    Error = $"Fehler beim übertragen der Daten. {e.Message}"
                };
            }
            return feedback;
        }

        // DEBUG
        public async Task SubmitDebugData(Run run)
        {
            string uri = App.API + "testrunitem";
            RunDebugModel data = new RunDebugModel()
            {
                Start = run.StartTime,
                Stop = run.StopTime,
                RunDebugItems = new List<RunDebugItemModel>(run.RunItems.Select(s => new RunDebugItemModel()
                {
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Accuracy = s.Accuracy,
                    TimeStamp = (double)s.TimeStamp.ToUnixTimeMilliseconds()
                })),
                Errors = new List<string>(run.RunItems.Where(x => !String.IsNullOrEmpty(x.Error)).Select(s => s.Error))
            };
            var json = JsonSerializer.Serialize(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(uri, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Debug post failed: {response.StatusCode}");
                }
            } catch (Exception e)
            {
                Console.WriteLine($"Debug post failed: {e.Message}");
            }
        }

        public async Task<Run> GetDebugData()
        {
            string uri = App.API + "gettestdata";
            var response = await client.GetAsync(uri);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            Run run = new Run();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                RunDebugModel debugModel = JsonSerializer.Deserialize<RunDebugModel>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                run = new Run()
                {
                    RunItems = debugModel.RunDebugItems.Select(s => new RunItem()
                    {
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        TimeStamp = new DateTime((long)s.TimeStamp).ToUniversalTime(),
                        Accuracy = s.Accuracy,
                        Speed = s.Speed
                    }).ToList()
                };
                run.StartTime = debugModel.Start;
                run.StopTime = debugModel.Stop;
            }
            return run;
        }
    }
}
