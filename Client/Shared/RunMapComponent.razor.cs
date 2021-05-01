
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) {
                // var mapUp = await _js.InvokeAsync<bool>("LoadMap", Run.RunItems.Select(s => new double[] { s.Latitude, s.Longitude }).ToArray());
                var mapUp = await _js.InvokeAsync<bool>("LoadMap");
                if (mapUp && Run.RunItems.Count > 2)
                    try {
                        await _js.InvokeVoidAsync("AddLine", Run.RunItems.Select(s => new double[] { s.Latitude, s.Longitude }).ToArray());
                    } catch (Exception e) {
                       _logger.LogError($"Filed drawing line: {e.Message}");
                    }
            }
        }
    }
}