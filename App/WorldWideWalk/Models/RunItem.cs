using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideWalk.Models
{
    public class RunItem
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public double Speed { get; set; }
        public double Angle { get; set; }
        public double Distance { get; set; }
        public TimeSpan TimeSpan { get; set; } = TimeSpan.Zero;
        public double SpeedInKmH => Distance == 0 || TimeSpan == TimeSpan.Zero ? 0 : (Distance / 1000) / TimeSpan.TotalHours;
        public string Error { get; set; }
        public RunItem() { }
        public RunItem(double latitude, double longitude, DateTimeOffset timestamp, double accuracy, double speed)
        {
            Latitude = latitude;
            Longitude = longitude;
            TimeStamp = timestamp;
            Accuracy = accuracy;
            Speed = speed;
        }

        public DateTime GetTime()
        {
            // TimeSpan time = TimeSpan.FromMilliseconds(TimeStamp);
            // return new DateTime(1970, 1, 1) + time;
            return TimeStamp.UtcDateTime;
        }
    }
}
