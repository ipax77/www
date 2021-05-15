using System;
using System.Collections.Generic;

namespace WorldWideWalk.Models
{
    public class LocationUpdatedEventArgs : EventArgs
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public double? Accuracy { get; }
        public DateTimeOffset Timestamp { get; }
        public string Error { get; }

        public LocationUpdatedEventArgs(double latitiude, double longitude, double? accuracy, DateTimeOffset timestamp, string error = "")
        {
            Latitude = latitiude;
            Longitude = longitude;
            Accuracy = accuracy;
            Timestamp = timestamp;
            Error = error;
        }
    }
}