
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using www.pwa.Client.Services;
using www.pwa.Shared;

namespace www.pwa.Client.Models
{
    public class Run
    {
        public List<RunItem> RunItems { get; set; } = new List<RunItem>();
        public RunInfo RunInfo { get; set; } = new RunInfo();
        public List<RunItem> MapItems { get; set; } = new List<RunItem>();

        public void GetRunInfo()
        {
            if (RunItems.Count > RunInfo.Pos + 3)
            {
                for (int i = RunInfo.Pos; i < RunItems.Count - 1; i++)
                {
                    double distance = GeoService.GetDistance(RunItems[i], RunItems[i + 1]);
                    RunInfo._Distance += distance;
                    if (distance > 0)
                        RunItems[i + 1].Speed = TimeSpan.FromSeconds(RunItems[i + 1].TimeStamp - RunItems[i].TimeStamp).TotalHours / (6376500.0 * distance);
                }
                RunInfo.Duration = RunItems.Last().GetTime() - RunInfo.StartTime;
                RunInfo.Pos = RunItems.Count - 1;
                RunInfo.KphMax = Math.Round(RunItems.Select(s => s.Speed).Max(), 2);
            }
        }

        public bool GetFinalRunInfo()
        {
            if (RunItems.Count < 6)
                return false;
            RunInfo.Duration = RunInfo.StopTime - RunInfo.StartTime;

            RunInfo._Distance = 0;

            for (int i = 3; i < RunItems.Count - 1; i++)
            {
                double distance = GeoService.GetDistance(RunItems[i], RunItems[i + 1]);
                RunInfo._Distance += distance;
                if (distance > 0)
                    RunItems[i + 1].Speed = TimeSpan.FromSeconds(RunItems[i + 1].TimeStamp - RunItems[i].TimeStamp).TotalHours / (6376500.0 * distance);
            }
            RunInfo.KphMax = Math.Round(RunItems.Select(s => s.Speed).Max(), 2);

            return true;
        }

        public void PrepareData(List<RunItem> runItems)
        {
            for (int i = 1; i < runItems.Count; i++)
            {
                // runItems[i].Angle = GeoService.angleFromCoordinate(runItems[i-1].Latitude, runItems[i-1].Longitude, runItems[i].Latitude, runItems[i].Longitude);
                runItems[i].Distance = GeoService.GetDistance(runItems[i - 1].Latitude, runItems[i - 1].Longitude, runItems[i].Latitude, runItems[i].Longitude);
                runItems[i].TimeSpan = runItems[i].GetTime() - runItems[i - 1].GetTime();
            }

        }

        public List<RunItem> FilterData()
        {
            PrepareData(RunItems);
            double averageAccuracy = RunItems.Select(s => s.Accuracy).Average();
            // double averageDistance = RunItems.Select(s => s.Distance).Average();
            // double averageTimespan = RunItems.Select(s => s.TimeSpan.TotalSeconds).Average();
            double averageSpeed = RunItems.Select(s => s.SpeedInKmH).Average();

            int toRemove = (int)(RunItems.Count * 0.15);
            var removeItems1 = RunItems.Where(x => x.Accuracy > averageAccuracy * 1.5).OrderByDescending(o => o.Accuracy).Take(toRemove);
            var removeItems2 = RunItems.Where(x => x.SpeedInKmH > averageSpeed * 1.5).OrderByDescending(o => o.SpeedInKmH).Take(toRemove);
            var removeItems = removeItems1.Union(removeItems2);
            var prepItems = RunItems.Except(removeItems).ToList();
            NoiseReduction(prepItems, 3);
            PrepareData(prepItems);
            return prepItems;
        }

        public void NoiseReduction(List<RunItem> runItems, int severity = 1)
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

        public List<ColorLine> GetLines()
        {
            List<ColorLine> lines = new List<ColorLine>();
            if (RunItems.Count < 3)
                return lines;

            lines.Add(new ColorLine()
            {
                Color = "blue",
                Line = new List<double[]>() {
                    new double[] { RunItems[0].Latitude, RunItems[0].Longitude },
                    new double[] { RunItems[1].Latitude, RunItems[1].Longitude }
                }
            });

            double avgSpeed = RunItems.Select(s => s.SpeedInKmH).Average();

            int i = 0;
            while (i < RunItems.Count)
            {
                string mode = GetMode(RunItems[i], avgSpeed);
                ColorLine line = new ColorLine()
                {
                    Color = mode,
                    Line = new List<double[]>() {
                        new double[] { RunItems[i].Latitude, RunItems[i].Longitude }
                    }
                };

                do
                {
                    i++;
                    if (i >= RunItems.Count)
                        break;
                    line.Line.Add(new double[] { RunItems[i].Latitude, RunItems[i].Longitude });
                } while (mode == GetMode(RunItems[i], avgSpeed));
                lines.Add(line);
            }

            lines.Add(new ColorLine()
            {
                Color = "blue",
                Line = new List<double[]>() {
                    new double[] { RunItems[RunItems.Count - 2].Latitude, RunItems[RunItems.Count - 2].Longitude },
                    new double[] { RunItems[RunItems.Count - 1].Latitude, RunItems[RunItems.Count - 1].Longitude }
                }
            });
            return lines;
        }

        private string GetMode(RunItem item, double avgSpeed)
        {
            string mode = "blue";
            if (item.SpeedInKmH < avgSpeed * 0.85)
                mode = "red";
            else if (item.SpeedInKmH > avgSpeed * 1.15)
                mode = "green";
            return mode;
        }
    }

    public class ColorLine
    {
        public string Color { get; set; }
        public List<double[]> Line { get; set; }
    }
}