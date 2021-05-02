using System;
namespace www.pwa.Client.Models
{
    public class RunInfo
    {
        public double Distance { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public double Kph => Math.Round(Distance / 1000 / Duration.TotalHours, 2);
        public int Pos { get; set; }
        public double KphMax { get; set; }
        public double KphAvg { get; set; }
    }
}