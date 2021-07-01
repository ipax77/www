
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
        private static Action<string> clickaction;
        TextEncoderSettings encoderSettings;
        PointInfoComponent pointInfoComponent;
        string pointInfo = String.Empty;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            action = MapLoaded;
            clickaction = PointClicked;
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
            if (Walk != null)
            {
                await _js.InvokeVoidAsync("AddLine", new List<double[]>(Walk.Points.Select(s => new double[] { s.Latitude, s.Longitude })), "blue");
                ShowCurrent();
                foreach (var point in Walk.Points)
                {
                    await _js.InvokeVoidAsync("AddMarker", point.Latitude, point.Longitude, point.Name);
                }
            }
        }

        private async void PointClicked(string point)
        {
            var wPoint = Walk.Points.FirstOrDefault(f => f.Name == point);
            pointInfo = point;
            if (wPoint != null)
            {
                await _js.InvokeVoidAsync("ClearLines");
                await _js.InvokeVoidAsync("FlyTo", wPoint.Latitude, wPoint.Longitude, 9);
                if (pointInfoComponent != null)
                {
                    await pointInfoComponent.LoadData(point);
                }
            }
            await InvokeAsync(() => StateHasChanged());
        }

        private void InfoShow(string point)
        {
            PointClicked(point);
        }

        private async void InfoClose()
        {
            pointInfo = String.Empty;
            await _js.InvokeVoidAsync("AddLine", new List<double[]>(Walk.Points.Select(s => new double[] { s.Latitude, s.Longitude })), "blue");
            ShowCurrent();
            await InvokeAsync(() => StateHasChanged());
        }

        [JSInvokable]
        public static void MapLoadedCaller()
        {
            action.Invoke();
        }

        [JSInvokable]
        public static void PointClick(string point)
        {
            clickaction.Invoke(point);
        }
    }
}