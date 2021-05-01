
using System;
namespace www.pwa.Client.Models {
    public class RunItem {
        public double Latitude {get; set;}
        public double Longitude {get; set;}
        public DateTime Time {get; set;}

        public RunItem() {}
        public RunItem(double latitude, double longitude) {
            Latitude = latitude;
            Longitude = longitude;
            Time = DateTime.UtcNow;
        }
    }
}