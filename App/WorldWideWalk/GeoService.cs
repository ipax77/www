using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorldWideWalk.Models;
using Xamarin.Essentials;

namespace WorldWideWalk
{
    public class GeoService : IDisposable
    {
        CancellationTokenSource cts;
        double zoomLevel = 13;
        double latlongDegrees => 360 / (Math.Pow(2, zoomLevel));
        public static event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        public async Task StartRun()
        {
            cts = new CancellationTokenSource();
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(1000, cts.Token);
                if (cts.IsCancellationRequested)
                    break;
                await GetCurrentLocation();
            }
        }

        public void StopRun()
        {
            cts.Cancel();
        }


        public async Task GetLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        public async Task GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                Location location = await Geolocation.GetLocationAsync(request, cts.Token);
                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Accuracy: {(location.Accuracy == null ? 0 : location.Accuracy)}");
                    LocationUpdated(this, new LocationUpdatedEventArgs(location.Latitude, location.Longitude, location.Accuracy, location.Timestamp));
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                LocationUpdated(this, new LocationUpdatedEventArgs(0, 0, 0, DateTimeOffset.MinValue, $"GeoTracking ist auf diesem Gerät nicht möglich. {fnsEx.Message}"));
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                LocationUpdated(this, new LocationUpdatedEventArgs(0, 0, 0, DateTimeOffset.MinValue, $"GeoTracking ist auf diesem Gerät nicht aktiviert. {fneEx.Message}"));
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                LocationUpdated(this, new LocationUpdatedEventArgs(0, 0, 0, DateTimeOffset.MinValue, $"GeoTracking ist auf diesem Gerät nicht erlaubt. {pEx.Message}"));
            }
            catch (Exception ex)
            {
                // Unable to get location
                LocationUpdated(this, new LocationUpdatedEventArgs(0, 0, 0, DateTimeOffset.MinValue, $"Fehler: {ex.Message}"));
            }
        }

        public static double GetDistance(double lat1, double long1, double lat2, double long2)
        {
            var d1 = lat1 * (Math.PI / 180.0);
            var num1 = long1 * (Math.PI / 180.0);
            var d2 = lat2 * (Math.PI / 180.0);
            var num2 = long2 * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            // return 2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3));
        }

        public void Dispose()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
        }
    }
}