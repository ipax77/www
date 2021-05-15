using System;
using System.Text.Json.Serialization;
using www.pwa.Shared;

namespace www.pwa.Client.Models
{
    public class RunInfo
    {
        public double _Distance {get; set;}
        [JsonIgnore]
        public double Distance => Math.Round(6376500.0 * _Distance, 2);
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public TimeSpan Duration { get; set; }
        [JsonIgnore]
        public double Kph => Math.Round(Distance / 1000 / Duration.TotalHours, 2);
        public int Pos { get; set; }
        public double KphMax { get; set; }
        public WwwFeedback Feedback { get; set; }
    }
}