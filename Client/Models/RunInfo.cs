using System;
using System.Text.Json.Serialization;
using www.pwa.Shared;

namespace www.pwa.Client.Models
{
    public class RunInfo
    {
        public double Distance { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        [JsonIgnore]
        public double Kph => Math.Round(Distance / 1000 / Duration.TotalHours, 2);
        public int Pos { get; set; }
        public double KphMax { get; set; }
        public double KphAvg { get; set; }
        public bool isDone { get; set; } = false;
        public WwwFeedback Feedback { get; set; }
        public bool isSubmit { get; set; } = false;
    }
}