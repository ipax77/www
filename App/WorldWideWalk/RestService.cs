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

        public async Task<List<Walk>> GetWalks()
        {
            Uri uri = new Uri(string.Format(App.API, "getwalks"));

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Walk>>(content);
                }
            } catch
            {

            }
            return new List<Walk>()
            {
                new Walk()
                {
                    Name = "WorldWideWalk"
                }
            };
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
            catch
            {
                
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
    }
}
