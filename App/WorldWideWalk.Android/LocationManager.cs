using Android.Gms.Location;
using Android.Util;
using System;
using System.Linq;
using WorldWideWalk.Droid;
using WorldWideWalk.Models;

[assembly: Xamarin.Forms.Dependency(typeof(LocationManager))]
namespace WorldWideWalk.Droid
{
    public class LocationManager : ILocationManager
    {
        LocationRequest locationRequest = new LocationRequest()
                                  .SetPriority(LocationRequest.PriorityHighAccuracy)
                                  .SetInterval(1000 * 2)
                                  .SetFastestInterval(1000 * 1);
        FusedLocationProviderCallback locationCallback;

        public async void StartLocationUpdates()
        {
            if (!MainActivity.isGooglePlayServicesInstalled)
            {
                LocationUpdated(null, new LocationUpdatedEventArgs(0, 0, 0, DateTimeOffset.MinValue, "Google Play Services ist nicht installiert."));
                return;
            }
            locationCallback = new FusedLocationProviderCallback(LocationUpdated);
            await MainActivity.fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, locationCallback);
        }

        private void GeoService_LocationUpdated(object sender, LocationUpdatedEventArgs e)
        {
            LocationUpdated(this, e);
        }

        public async void StopLocationUpdates()
        {
            await MainActivity.fusedLocationProviderClient.RemoveLocationUpdatesAsync(locationCallback);
        }

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

    }

    public class FusedLocationProviderCallback : LocationCallback
    {
        private event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        public FusedLocationProviderCallback(EventHandler<LocationUpdatedEventArgs> locationUpdated)
        {
            LocationUpdated = locationUpdated;
        }

        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            Log.Debug("FusedLocationProviderSample", "IsLocationAvailable: {0}", locationAvailability.IsLocationAvailable);
        }

        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Any())
            {
                var location = result.Locations.First();
                Log.Debug("Sample", "The latitude is :" + location.Latitude);
                LocationUpdated(this, new LocationUpdatedEventArgs(location.Latitude, location.Longitude, location.Accuracy, UnixTimeStampToDateTime(location.Time)));
            }
            else
            {
                LocationUpdated(this, new LocationUpdatedEventArgs(0, 0, 0, DateTimeOffset.MinValue, "No data available."));
            }
        }

        private DateTimeOffset UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }
    }
}