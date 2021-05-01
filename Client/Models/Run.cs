
using System;
using System.Collections.Generic;
using System.Linq;

namespace www.pwa.Client.Models
{
    public class Run
    {
        public List<RunItem> RunItems { get; set; } = new List<RunItem>();
        public RunInfo RunInfo {get; set;} = new RunInfo();

        public void GetRunInfo()
        {
            if (RunItems.Count > RunInfo.Pos + 2)
            {
                for (int i = RunInfo.Pos; i < RunItems.Count - 1; i++)
                {
                    RunInfo.Distance += GetDistance(RunItems[i], RunItems[i+1]);
                }
                RunInfo.Duration = RunItems.Last().Time - RunItems.First().Time;
                RunInfo.Pos = RunItems.Count - 1;
            }
        }

        public double GetDistance(RunItem item1, RunItem item2)
        {
            var d1 = item1.Latitude * (Math.PI / 180.0);
            var num1 = item1.Longitude * (Math.PI / 180.0);
            var d2 = item2.Latitude * (Math.PI / 180.0);
            var num2 = item2.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}