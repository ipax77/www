
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
                    RunInfo._Distance += GeoService.GetDistance(RunItems[i], RunItems[i + 1]);
                }
                RunInfo.StartTime = RunItems.First().GetTime();
                RunInfo.Duration = RunItems.Last().GetTime() - RunItems.First().GetTime();
                RunInfo.Pos = RunItems.Count - 1;
                RunInfo.KphAvg = Math.Round(RunItems.Select(s => s.Speed).Average(), 2);
                RunInfo.KphMax = Math.Round(RunItems.Select(s => s.Speed).Max(), 2);
            }
        }

        public bool GetFinalRunInfo()
        {
            if (RunItems.Count < 6)
                return false;
            RunInfo.StartTime = RunItems.First().GetTime();
            RunInfo.Duration = RunItems.Last().GetTime() - RunItems.First().GetTime();

            RunInfo._Distance = 0;

            for (int i = 3; i < RunItems.Count - 1; i++)
            {
                RunInfo._Distance += GeoService.GetDistance(RunItems[i], RunItems[i + 1]);
            }
            RunInfo.KphAvg = Math.Round(RunItems.Select(s => s.Speed).Average(), 2);
            RunInfo.KphMax = Math.Round(RunItems.Select(s => s.Speed).Max(), 2);

            return true;
        }


    }
}