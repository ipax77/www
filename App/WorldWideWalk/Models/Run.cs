using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace WorldWideWalk.Models
{
    public class Run
    {
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public List<RunItem> RunItems { get; set; } = new List<RunItem>();
        public double Distance { get; set; }
        [JsonIgnore]
        public double AverageSpeedInKmH => (Distance / 1000) / (StopTime - StartTime).TotalHours;
        [JsonIgnore]
        public HtmlWebViewSource Html;
        public string SetRunInfo()
        {
            if (RunItems.Count < 6)
                return "Zu wenige Daten";

            RunItems = FilterData();
            Distance = Math.Round(RunItems.Select(s => s.Distance).Sum(), 2);
            if (AverageSpeedInKmH > App.MaxSpeedInKmH)
                return "Maximale Höchstgeschwindigkeit überschritten.";
            Html = SetWebViewSource();
            return "";
        }

        private List<RunItem> FilterData()
        {
            var runItems = RunItems.Where(x => String.IsNullOrEmpty(x.Error)).ToList();
            PrepareData(runItems);
            double averageAccuracy = runItems.Select(s => s.Accuracy).Average();
            // double averageDistance = runItems.Select(s => s.Distance).Average();
            // double averageTimespan = runItems.Select(s => s.TimeSpan.TotalSeconds).Average();
            double averageSpeed = runItems.Select(s => s.SpeedInKmH).Average();

            int toRemove = (int)(runItems.Count * 0.15);
            var removeItems1 = runItems.Where(x => x.Accuracy > averageAccuracy * 1.5).OrderByDescending(o => o.Accuracy).Take(toRemove);
            var removeItems2 = runItems.Where(x => x.SpeedInKmH > averageSpeed * 1.5).OrderByDescending(o => o.SpeedInKmH).Take(toRemove);
            var removeItems = removeItems1.Union(removeItems2);
            var prepItems = runItems.Except(removeItems).ToList();
            NoiseReduction(prepItems, 3);
            PrepareData(prepItems);
            return prepItems;
        }

        private void PrepareData(List<RunItem> runItems)
        {
            for (int i = 1; i < runItems.Count; i++)
            {
                // runItems[i].Angle = GeoService.angleFromCoordinate(runItems[i-1].Latitude, runItems[i-1].Longitude, runItems[i].Latitude, runItems[i].Longitude);
                runItems[i].Distance = GeoService.GetDistance(runItems[i - 1].Latitude, runItems[i - 1].Longitude, runItems[i].Latitude, runItems[i].Longitude);
                runItems[i].TimeSpan = runItems[i].GetTime() - runItems[i - 1].GetTime();
            }

        }

        private void NoiseReduction(List<RunItem> runItems, int severity = 1)
        {
            for (int i = 1; i < runItems.Count; i++)
            {
                //---------------------------------------------------------------avg
                var start = (i - severity > 0 ? i - severity : 0);
                var end = (i + severity < runItems.Count ? i + severity : runItems.Count);

                double latsum = 0;
                double lonsum = 0;

                for (int j = start; j < end; j++)
                {
                    latsum += runItems[j].Latitude;
                    lonsum += runItems[j].Longitude;
                }

                var latavg = latsum / (end - start);
                var lonavg = lonsum / (end - start);

                runItems[i].Latitude = latavg;
                runItems[i].Longitude = lonavg;
            }
        }

        private ColorLine GetMainLine()
        {
            return new ColorLine()
            {
                Color = "blue",
                Line = new List<double[]>(RunItems.Where(x => String.IsNullOrEmpty(x.Error)).Select(s => new double[] {
                        s.Latitude,
                        s.Longitude
                    }))
            };
        }

        private HtmlWebViewSource SetWebViewSource()
        {
            var mainline = GetMainLine();
            return new HtmlWebViewSource
            {
                Html = @"<html>
     <head>
    <meta charset= ""UTF-8"">
     <link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.7.1/dist/leaflet.css""
   integrity = ""sha512-xodZBNTC5n17Xt2atTPuE1HxjVMSvLVW9ocqUKLsCC5CXdbqCmblAshOMAS6/keqq/sMZMZ19scR4PsZChSR7A==""
   crossorigin = """" />
     <script src=""https://unpkg.com/leaflet@1.7.1/dist/leaflet.js""
   integrity = ""sha512-XQoYMqMTK8LvdxXYG3nZ448hOEQiglfqkJs1NOQV44cWnUrBc8PkAOcXy20w0vlaXaVUearIOBhiXZ5V3ynxwA==""
   crossorigin = """" ></script>
    <script>
    function init() {
                    var map = L.map('mapcontainer');
                    map.setView([51.318008, 9.468067], 6);
                    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                        attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors'
                    }).addTo(map);
    " +
    " var polyline = L.polyline(" +
        mainline.GetJsonString() +
    ", { color: '" + mainline.Color + "' }).addTo(map);" +
    " map.fitBounds(polyline.getBounds());" +
    "}" +
    "</script>" +
    "</head>" +
    "<body onload = \"init()\" >" +
    "<div id=\"mapcontainer\" style=\"width: " + DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density + "px; height: " + "500" + "px\"></div>" +
    "</body>" +
    "</html>",

            };
        }
    }
}
