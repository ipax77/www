
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using www.pwa.Client.Models;

namespace www.pwa.Client.Pages
{
    public partial class RunPage : ComponentBase, IDisposable
    {
        [Inject]
        protected IJSRuntime _js { get; set; }
        [Inject]
        protected ILogger<RunPage> _logger { get; set; }
        private static Action<double[]> action;
        private int watchId;
        private Run Run = new Run();
        private bool runDone = false;

        List<RunItem> testItems = new List<RunItem>() {
            new RunItem(48.3351, 10.8579),
            new RunItem(48.3352, 10.8578),
            new RunItem(48.3353, 10.8577),
            new RunItem(48.3354, 10.8576),
            new RunItem(48.3355, 10.8575),
            new RunItem(48.3356, 10.8574),
            new RunItem(48.3357, 10.8573),
            new RunItem(48.3358, 10.8572),
            new RunItem(48.3359, 10.8571),
            new RunItem(48.3360, 10.8560),
            new RunItem(48.3361, 10.8569),
            new RunItem(48.3362, 10.8568),
            new RunItem(48.3363, 10.8567),
            new RunItem(48.3364, 10.8566),
            new RunItem(48.3365, 10.8565),
            new RunItem(48.3366, 10.8564),
            new RunItem(48.3367, 10.8563),
            new RunItem(48.33716, 10.85624),
        };

        protected override void OnInitialized()
        {
            action = UpdateRun;
        }

        private void UpdateRun(double[] pos) {
            double latitude = pos[0];
            double longitude = pos[1];
            if (latitude > 0)
                Run.RunItems.Add(new RunItem(latitude, longitude));
            _logger.LogInformation($"1: {latitude}, 2: {longitude}");
        }

        [JSInvokable]
        public static void UpdateRunCaller(double[] pos) {
            action.Invoke(pos);
        }

        private async Task GetLocation()
        {
            var data = await _js.InvokeAsync<string>("GetLocation");
            _logger.LogInformation(data);
        }

        private async Task StartRun() {
            string start = await _js.InvokeAsync<string>("StartRun");
            int id;
            if (int.TryParse(start, out id)) {
                watchId = id;
                runDone = false;
                await InvokeAsync(() => StateHasChanged());
            }
            else
                _logger.LogError($"Failed starting run: {start}");
        }

        public async Task StopRun(bool dispose = false) {
            // testItems.First().Time = DateTime.UtcNow.AddMinutes(-4);
            // testItems.Last().Time = DateTime.UtcNow;
            // Run.RunItems = testItems;
            Run.GetRunInfo();
            if (watchId > 0) {
                await _js.InvokeVoidAsync("StopRun", watchId);
                if (!dispose) {
                    watchId = 0;
                    runDone = true;
                    await InvokeAsync(() => StateHasChanged());
                }
            }
        }

        public async void Dispose() {
            await StopRun(true);
        }
        
    }
}