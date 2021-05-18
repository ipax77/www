
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
        protected IHttpClientFactory httpClient { get; set; }
        [Inject]
        protected IJSRuntime _js { get; set; }
        [Inject]
        protected ILogger<MapComponent> logger { get; set; }

        [Parameter]
        public string Guid { get; set; } = "7A40C465-BDC8-4373-B6BE-6E49C10D5ECA";

        WalkAppModel Walk;
        private static Action action;
        TextEncoderSettings encoderSettings;
        double CurrentDistance;

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
                var http = httpClient.CreateClient("www.pwa.ServerAPI.NoAuth");

                try
                {
                    Walk = await http.GetFromJsonAsync<WalkAppModel>($"WwwRun/walk/{Guid}",
                        new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true,
                            Encoder = JavaScriptEncoder.Create(encoderSettings)
                        });

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Walk get error: {e.Message}");
                }
                await _js.InvokeVoidAsync("LoadMap");
            }
        }

        private void ShowCurrent()
        {
            Walk.CurrentDistance = CurrentDistance;
            (var next, var current) = Walk.GetNextAndCurrentPoint();
            Walk.NextTarget = next.Name;
            List<double[]> line = new List<double[]>();
            line.AddRange(Walk.Points.Where(x => x.Distance < Walk.CurrentDistance).Select(s => new double[] { s.Latitude, s.Longitude }));
            line.Add(new double[] { current.Latitude, current.Longitude });
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