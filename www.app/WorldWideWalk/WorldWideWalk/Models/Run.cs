using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorldWideWalk.Models
{
    public class Run
    {
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public List<RunItem> RunItems { get; set; } = new List<RunItem>();
        public double Distance { get; set; }
        [JsonIgnore]
        public double AverageSpeedInKmH => Math.Round((Distance / 1000) / (StopTime - StartTime).TotalHours, 2);
        [JsonIgnore]
        public HtmlWebViewSource Html;
        public string SetRunInfo()
        {
            if (RunItems.Count < 6)
                return "Zu wenige Daten";

            RunItems = FilterData();
            Distance = Math.Round(RunItems.Select(s => s.Distance).Sum(), 2);

            // Distance = 1001;

            if (AverageSpeedInKmH > App.MaxSpeedInKmH)
                return "Maximale Höchstgeschwindigkeit überschritten.";

            Html = SetWebViewSource();
            return "";
        }

        public double GetStepInfo()
        {
            if (RunItems.Count < 6)
                return 0;
            var items = FilterData();
            return Math.Round(items.Select(s => s.Distance).Sum(), 2);
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
                runItems[i].Distance = GetDistance(runItems[i - 1].Latitude, runItems[i - 1].Longitude, runItems[i].Latitude, runItems[i].Longitude);
                runItems[i].TimeSpan = runItems[i].GetTime() - runItems[i - 1].GetTime();
            }

        }

        private void NoiseReduction(List<RunItem> runItems, int severity = 1)
        {
            for (int i = 1; i < runItems.Count; i++)
            {
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
            var json = mainline.GetJsonString();
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
        json +
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

        public static double GetDistance(double lat1, double long1, double lat2, double long2)
        {
            var d1 = lat1 * (Math.PI / 180.0);
            var num1 = long1 * (Math.PI / 180.0);
            var d2 = lat2 * (Math.PI / 180.0);
            var num2 = long2 * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            // return 2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3));
        }
        public static DateTimeOffset UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }
    }
}
