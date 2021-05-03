
using System.Net.Http;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using www.pwa.Client.Models;
using System.Net.Http.Json;
using www.pwa.Shared;
using www.pwa.Client.Services;

namespace www.pwa.Client.Pages
{
    public partial class RunPage : ComponentBase, IDisposable
    {
        [Inject]
        protected IJSRuntime _js { get; set; }
        [Inject]
        protected ILogger<RunPage> _logger { get; set; }
        [Inject]
        protected HttpClient Http { get; set; }
        [Inject]
        protected DataService dataService { get; set; }
        private static Action<double[]> action;
        private int watchId;
        private Run Run = new Run();
        private string debugInfo = String.Empty;
        private UploadStatus UploadStatus = UploadStatus.UploadDone;

        protected override void OnInitialized()
        {
            action = UpdateRun;
        }

        private void UpdateRun(double[] pos)
        {

            double latitude = pos[0];
            double longitude = pos[1];
            double timestamp = pos[2];
            double accuracy = pos[3];
            double speed = pos[4];

            if (latitude != 0)
                Run.RunItems.Add(new RunItem(latitude, longitude, timestamp, accuracy, speed));
            // debugInfo = $"1: {latitude}, 2: {longitude}, 3: {timestamp}, 4: {accuracy}, 5: {speed}";
            // _logger.LogInformation(debugInfo);                
            // InvokeAsync(() => StateHasChanged());
        }

        [JSInvokable]
        public static void UpdateRunCaller(double[] pos)
        {
            action.Invoke(pos);
        }

        private async Task GetLocation()
        {
            var data = await _js.InvokeAsync<string>("GetLocation");
            _logger.LogInformation(data);
        }

        private async Task StartRun()
        {
            Run = new Run();
            string start = await _js.InvokeAsync<string>("StartRun");
            int id;
            if (int.TryParse(start, out id))
            {
                watchId = id;
                await InvokeAsync(() => StateHasChanged());
            }
            else
                _logger.LogError($"Failed starting run: {start}");
        }

        private async Task DebugRun()
        {
            watchId = 77;
            var data = await Http.GetFromJsonAsync<List<double[]>>("api/testrun");
            foreach (var d in data)
            {
                Run.RunItems.Add(new RunItem(d[0], d[1], d[3], d[2], d[4]));
            }
            await StopRun();
        }

        public async Task StopRun(bool dispose = false)
        {

            Run.GetFinalRunInfo();
            if (watchId > 0)
            {
                await _js.InvokeVoidAsync("StopRun", watchId);
                if (!dispose)
                {
                    watchId = 0;
                    Run.RunInfo.isDone = true;
                    await SubmitData();
                }
            }
        }

        private async Task SubmitData()
        {
            UploadStatus = UploadStatus.Uploading;
            await InvokeAsync(() => StateHasChanged());
            EntityRunFormData data = new EntityRunFormData()
            {
                School = "TestSchool",
                Walk = "Weltumrundung",
                Identifier = "Anonymouse",
                SchoolClass = "Lehrer",
                Distance = (float)Run.RunInfo.Distance,
                Time = Run.RunInfo.StartTime,
                Credential = "test123"
            };
            Run.RunInfo.Feedback = await dataService.Submit(data);
            if (Run.RunInfo.Feedback == null)
                UploadStatus = UploadStatus.UploadFailed;
            else
                UploadStatus = UploadStatus.UploadSuccess;
            await InvokeAsync(() => StateHasChanged());
        }

        private void RunInfo()
        {
            Run.GetRunInfo();
            StateHasChanged();
        }

        public async void Dispose()
        {
            await StopRun(true);
        }

    }
}