using System;
namespace www.pwa.Client.Models
{
    public class RunInfo
    {
        public double Distance {get; set;}
        public TimeSpan Duration {get; set;}
        public double Kph => Distance / 1000 / Duration.TotalHours;
        public int Pos {get; set;}
    }
}