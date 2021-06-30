
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using www.pwa.Shared;
using System;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;

namespace www.pwa.Client.Shared
{
    public partial class MapComponent : ComponentBase
    {
        [Inject]
        protected IJSRuntime _js { get; set; }
        [Inject]
        protected ILogger<MapComponent> logger { get; set; }

        [Parameter]
        public WalkAppModel Walk { get; set; }
        private static Action action;
        TextEncoderSettings encoderSettings;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            action = MapLoaded;
            encoderSettings = new TextEncoderSettings();
            encoderSettings.AllowRange(UnicodeRanges.All);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _js.InvokeVoidAsync("LoadMap");
            }
        }

        public void ShowCurrent(WwwFeedback feedback = null)
        {
            List<double[]> line = new List<double[]>();
            line.AddRange(Walk.Points.Where(x => x.Distance < Walk.CurrentDistance).Select(s => new double[] { s.Latitude, s.Longitude }));
            line.Add(new double[] { Walk.CurrentPoint.Latitude, Walk.CurrentPoint.Longitude });
            _js.InvokeVoidAsync("AddCurrentLine", line, "purple");
            InvokeAsync(() => StateHasChanged());
        }

        private async void MapLoaded()
        {
            if (Walk != null) {
                await _js.InvokeVoidAsync("AddLine", new List<double[]>(Walk.Points.Select(s => new double[] { s.Latitude, s.Longitude })), "blue");
                ShowCurrent();
            }
        }

        [JSInvokable]
        public static void MapLoadedCaller()
        {
            action.Invoke();
        }
    }
}