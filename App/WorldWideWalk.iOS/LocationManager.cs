using CoreLocation;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using WorldWideWalk.iOS;
using WorldWideWalk.Models;

[assembly: Xamarin.Forms.Dependency(typeof(LocationManager))]
namespace WorldWideWalk.iOS
{
    public class LocationManager : ILocationManager
    {
        protected CLLocationManager locMgr;
        private bool isRunning = false;

        public LocationManager()
        {
            this.locMgr = new CLLocationManager();
            this.locMgr.PausesLocationUpdatesAutomatically = false;

            // iOS 8 has additional permissions requirements
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization(); // works in background
                //locMgr.RequestWhenInUseAuthorization (); // only in foreground
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locMgr.AllowsBackgroundLocationUpdates = true;
            }
        }

        public CLLocationManager LocMgr
        {
            get { return this.locMgr; }
        }

        public void StartLocationUpdates()
        {
            if (CLLocationManager.LocationServicesEnabled && !isRunning)
            {
                isRunning = true;
                //set the desired accuracy, in meters
                LocMgr.DesiredAccuracy = 1;
                LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    // fire our custom Location Updated event
                    // LocationUpdated(this, new LocationUpdatedEventArgs(new KeyValuePair<double, double>(e.Locations[e.Locations.Length - 1].Coordinate.Latitude, e.Locations[e.Locations.Length - 1].Coordinate.Longitude)));
                    var location = e.Locations[e.Locations.Length - 1];
                    LocationUpdated(this, new LocationUpdatedEventArgs(location.Coordinate.Latitude, location.Coordinate.Longitude, location.CourseAccuracy, NSDateToDateTimeOffset(location.Timestamp)));
                };
                LocMgr.StartUpdatingLocation();
            }
        }

        public DateTimeOffset NSDateToDateTimeOffset(NSDate date)
        {
            DateTime time = (DateTime)date;
            DateTimeOffset offset = time;
            return offset;
        }

        public void StopLocationUpdates()
        {
            if (isRunning)
            {
                LocMgr.StopUpdatingLocation();
                LocMgr.Dispose();
                isRunning = false;
            }
        }

        // event for the location changing
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
    }
}