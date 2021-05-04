
using System.Net.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using www.pwa.Client.Models;
using System.Net.Http.Json;

namespace www.pwa.Client.Shared
{
    public partial class RunMapComponent : ComponentBase
    {
        [Inject]
        protected IJSRuntime _js { get; set; }
        [Inject]
        protected ILogger<RunMapComponent> _logger { get; set; }
        [Inject]
        protected HttpClient Http { get; set; }

        [Parameter]
        public Run Run { get; set; }
        private static Action action;
        private bool mapUp;

        protected override void OnInitialized()
        {
            action = MapLoaded;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // var mapUp = await _js.InvokeAsync<bool>("LoadMap", Run.RunItems.Select(s => new double[] { s.Latitude, s.Longitude }).ToArray());
                mapUp = await _js.InvokeAsync<bool>("LoadMap");
                // DEBUG
                await Http.PostAsJsonAsync("api/testdata", Run.RunItems.Select(s => new double[] { s.Latitude, s.Longitude, s.Accuracy, s.TimeStamp, s.Speed }).ToList());
            }
        }

        private async void MapLoaded()
        {
            if (Run.RunItems.Count > 3)
                try
                {
                    // double sdiff = Run.RunInfo.KphMax - Run.RunInfo.KphAvg;
                    // var slow = Run.RunItems.Where(x => x.Speed < (Run.RunInfo.KphAvg - (sdiff * 0.3))).Select(s => new double[] { s.Latitude, s.Longitude }).ToList();
                    // var avg = Run.RunItems.Where(x => x.Speed > (Run.RunInfo.KphAvg - (sdiff * 0.3)) && x.Speed < (Run.RunInfo.KphAvg + (sdiff * 0.3))).Select(s => new double[] { s.Latitude, s.Longitude }).ToList();
                    // var fast = Run.RunItems.Where(x => x.Speed > (Run.RunInfo.KphAvg + (sdiff * 0.3))).Select(s => new double[] { s.Latitude, s.Longitude }).ToList();
                    // await _js.InvokeVoidAsync("AddLine", slow, "red");
                    // await _js.InvokeVoidAsync("AddLine", avg, "yellow");
                    // await _js.InvokeVoidAsync("AddLine", fast, "green");
                    
                    
                    await _js.InvokeVoidAsync("AddLine", Run.RunItems.Select(s => new double[] { s.Latitude, s.Longitude }).ToList(), "red");

                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed drawing line: {e.Message}");
                }
        }


        [JSInvokable]
        public static void MapLoadedCaller()
        {
            action.Invoke();
        }

    }
}