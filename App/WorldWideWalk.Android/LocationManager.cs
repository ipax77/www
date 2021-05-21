using System;
using WorldWideWalk.Droid;
using WorldWideWalk.Models;

[assembly: Xamarin.Forms.Dependency(typeof(LocationManager))]
namespace WorldWideWalk.Droid
{
    public class LocationManager : ILocationManager
    {
        IAndroidService androidService;
        public void StartLocationUpdates()
        {
            androidService = new AndroidServiceHelper();
            androidService.StartService();
            GeoService.LocationUpdated += GeoService_LocationUpdated; ;
        }

        private void GeoService_LocationUpdated(object sender, LocationUpdatedEventArgs e)
        {
            LocationUpdated(this, e);
        }

        public void StopLocationUpdates()
        {
            androidService.StopService();
            GeoService.LocationUpdated -= GeoService_LocationUpdated;
        }

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

    }
}