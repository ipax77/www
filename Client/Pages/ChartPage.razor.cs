using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using www.pwa.Client.Services;

namespace www.pwa.Client.Pages
{
    public partial class ChartPage : ComponentBase
    {
        [Inject]
        protected DataService dataService { get; set; }
        [Inject]
        protected IJSRuntime js { get; set; }

        BarConfig _config;
        Chart _chart;
        public string firstActive = "active";
        int minheight = 400;
        int width = 540;
        bool Reset = true;

        protected override void OnInitialized()
        {
            SetConfig();
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                await ChangeMode();
            }
        }

        public void SetConfig()
        {
            _config = new BarConfig(horizontal: true)
            {
                Options = new BarOptions()
                {
                    Responsive = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Padding = 5,
                        FontSize = 20,
                        Text = "Loading ...",
                        FontColor = "#f2f2f2"
                    },
                    Scales = new BarScales()
                    {
                        YAxes = new List<CartesianAxis>()
                        {
                            new BarCategoryAxis
                            {
                                GridLines = new GridLines()
                                {
                                    Color = "#404040",
                                    BorderDash = new double[] {3, 11}
                                },
                                Ticks = new CategoryTicks()
                                {
                                    FontColor = "#c9bd14"
                                },
                                ScaleLabel = new ScaleLabel()
                                {
                                    LabelString = "Jahrgänge"
                                }
                                // BarThickness = BarThickness.Absolute(50.0),
                                // Offset = false
                            }
                        },
                        XAxes = new List<CartesianAxis>
                        {

                            new LinearCartesianAxis
                            {
                                GridLines = new GridLines()
                                {
                                    Color = "#404040",
                                    BorderDash = new double[] {3, 11}
                                },
                                Ticks = new LinearCartesianTicks()
                                {
                                    BeginAtZero = true,
                                    FontColor = "#c9bd14"
                                },
                                ScaleLabel = new ScaleLabel()
                                {
                                    LabelString = "km"
                                },
                            }
                        }
                    }
                }
            };
        }

        public async Task SetWidth() {
            width = await js.InvokeAsync<int>("GetWidth", "wsize");
            width = (int)(width * 0.8);
            if (width > 900)
                width = 900;
            else if (width < 400)
                width = 400;
        }

        public async Task ChangeMode(string mode = "Years")
        {
            var data = await dataService.GetChartData(mode);
            await SetWidth();
            //if (mode == "Years")
            //{
            //    data.Lables.AddRange(Enumerable.Repeat("test", 20));
            //    data.Data.AddRange(Enumerable.Repeat(20.2, 20));
            //}

            minheight = data.Data.Count * 40 + 100;
            if (minheight < 400)
                minheight = 400;

            await InvokeAsync(() => StateHasChanged());
            Reset = true;
            _chart = null;
            _config = null;
            await InvokeAsync(() => StateHasChanged());
            await Task.Delay(25);
            SetConfig();
            Reset = false;
            await InvokeAsync(() => StateHasChanged());

            if (data == null)
            {
                _config.Options.Title.Text = "Keine Daten verfügbar.";
                _config.Options.Scales.XAxes.First().ScaleLabel.LabelString = "no data";
                await _chart.Update();
                return;
            }

            string Title = mode switch
            {
                "Years" => "Jahrgänge",
                "Classes" => "Klassen",
                _ => "Loading ..."
            };
            _config.Options.Title.Text = Title;
            // _config.Options.Scales.YAxes.First().ScaleLabel.LabelString = "Title";
            foreach (var label in data.Lables)
                _config.Data.Labels.Add(label);

            BarDataset<double> dataset = new BarDataset<double>(data.Data, horizontal: true)
            {
                BackgroundColor = Enumerable.Repeat("rgba(87, 66, 245, 0.8)", data.Data.Count).ToArray(),
                Label = "",
                BorderWidth = 1,
                BorderColor = "rgba(255, 0, 0, 1)",
            };

            _config.Data.Datasets.Add(dataset);
            await _chart.Update();
            firstActive = "";
        }
    }
}
