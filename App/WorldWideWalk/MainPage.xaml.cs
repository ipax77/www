using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldWideWalk.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorldWideWalk
{
    public partial class MainPage : ContentPage
    {
        private ILocationManager locationManager;
        private IRestService restService;
        private Run Run = new Run();

        public MainPage()
        {
            InitializeComponent();
            Debug();
        }

        private async void Debug()
        {
            restService = new RestService();
            Run = await restService.GetDebugData();
            string finfo = Run.SetRunInfo();
            if (!String.IsNullOrEmpty(finfo))
            {
                LbTime.Text = finfo;
                return;
            }
            MapWebView wvMap = new MapWebView()
            {
                Source = Run.Html,
                HeightRequest = 500,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            grMap.Children.Add(wvMap);
            MapLayout.IsVisible = true;
        }

        private void StartRun(object sender, EventArgs e)
        {
            LbTime.Text = "";
            LbDistance.Text = "";
            LbCount.Text = "";
            BtnStart.IsEnabled = false;
            BtnStart.Clicked -= StartRun;
            BtnStop.IsEnabled = true;
            BtnStop.Clicked += StopRun;
            Run = new Run();
            Run.StartTime = DateTime.UtcNow;
            locationManager = DependencyService.Get<ILocationManager>();
            locationManager.StartLocationUpdates();
            locationManager.LocationUpdated += LocationManager_LocationUpdated;
        }

        private async void StopRun(object sender, EventArgs e)
        {
            BtnStart.IsEnabled = true;
            BtnStart.Clicked += StartRun;
            BtnStop.IsEnabled = false;
            BtnStop.Clicked -= StopRun;
            locationManager.StopLocationUpdates();
            locationManager.LocationUpdated -= LocationManager_LocationUpdated;
            Run.StopTime = DateTime.UtcNow;
            restService = new RestService();

            if (sender != null)
            {
                LbTime.Text = $"Laufzeit: {(Run.StopTime - Run.StartTime).ToString(@"hh\:mm\:ss")}";
            }
            LbDistance.Text = "Zurückgelegt: " + Math.Round(Run.Distance, 2).ToString() + " m";

            // DEBUG
            await restService.SubmitDebugData(Run);
            LbCount.Text = Run.RunItems.Count.ToString();
        }

        private void LocationManager_LocationUpdated(object sender, Models.LocationUpdatedEventArgs e)
        {
            if (Run.RunItems.Count == 0 && !String.IsNullOrEmpty(e.Error))
            {
                StopRun(null, null);
                LbTime.Text = e.Error;
                return;
            } else
            {
                Run.RunItems.Add(new RunItem()
                {
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    Accuracy = e.Accuracy == null ? 0 : (double)e.Accuracy,
                    TimeStamp = e.Timestamp,
                    Error = e.Error
                });
                
                if (Run.RunItems.Count % 10 == 0)
                {
                    Run.SetRunInfo();
                    if ((DateTime.UtcNow - Run.StartTime) > App.MaxRunTime)
                    {
                        StopRun(null, null);
                        LbTime.Text = "Der Lauf wurde nach 2h gestoppt.";
                        return;
                    }

                    if (Run.AverageSpeedInKmH > App.MaxSpeedInKmH)
                    {
                        StopRun(null, null);
                        LbTime.Text = "Die maximal erlaubte Geschwindigkeit wurde überschritten.";
                        return;
                    }
                    LbTime.Text = $"Laufzeit: {(DateTime.UtcNow - Run.StartTime).ToString(@"hh\:mm\:ss")}";
                    LbDistance.Text = "Zurückgelegt: " + Math.Round(Run.Distance, 2).ToString() + " m";
                }
            }
        }
    }
}
