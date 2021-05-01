
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using www.pwa.Client.Models;

namespace www.pwa.Client.Shared
{
    public partial class RunMapComponent : ComponentBase
    {
        [Inject]
        protected IJSRuntime _js { get; set; }
        [Inject]
        protected ILogger<RunMapComponent> _logger {get; set;}

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
            if (firstRender) {
                // var mapUp = await _js.InvokeAsync<bool>("LoadMap", Run.RunItems.Select(s => new double[] { s.Latitude, s.Longitude }).ToArray());
                mapUp = await _js.InvokeAsync<bool>("LoadMap");

            }
        }

        private async void MapLoaded() {
            if (Run.RunItems.Count > 3)
                try {
                    // await _js.InvokeVoidAsync("AddLine", Run.GetMapItems());
                    await _js.InvokeVoidAsync("AddLine", Run.RunItems.Select(s => new double[] { s.Latitude, s.Longitude }).ToList());
                } catch (Exception e) {
                    _logger.LogError($"Failed drawing line: {e.Message}");
                }
        }

        
        [JSInvokable]
        public static void MapLoadedCaller() {
            action.Invoke();
        }

    }
}