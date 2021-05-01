
using System;
using System.Collections.Generic;
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