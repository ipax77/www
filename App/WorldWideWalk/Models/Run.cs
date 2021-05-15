using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WorldWideWalk.Models
{
    public class Run
    {
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public List<RunItem> RunItems { get; set; } = new List<RunItem>();
        public double Distance { get; set; }
        public double AverageSpeedInKmH => (Distance / 1000) / (StopTime - StartTime).TotalHours;

        public bool SetRunInfo()
        {
            if (RunItems.Count < 6)
                return false;

            var items = FilterData();
            Distance = Math.Round(items.Select(s => s.Distance).Sum(), 2);
            return true;
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
    }
}
