
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using www.pwa.Client.Models;

namespace www.pwa.Client.Services
{
    public class GeoService
    {
        public static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public static async Task GetLocationAsync(IJSRuntime js) {
            await semaphoreSlim.WaitAsync();
            
            try {
                await js.InvokeVoidAsync("GetLocation");
            } catch (Exception e) {

            } finally {
                semaphoreSlim.Release();
            }
        }

        public static double GetDistance(RunItem item1, RunItem item2)
        {
            var d1 = item1.Latitude * (Math.PI / 180.0);
            var num1 = item1.Longitude * (Math.PI / 180.0);
            var d2 = item2.Latitude * (Math.PI / 180.0);
            var num2 = item2.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            // return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            return 2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3));
        }

        public static double angleFromCoordinate(double lat1, double long1, double lat2,
            double long2) {

            double dLon = (long2 - long1);

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1)
                    * Math.Cos(lat2) * Math.Cos(dLon);

            double brng = Math.Atan2(y, x);

            brng = (180 / Math.PI) * brng;
            brng = (brng + 360) % 360;
            // brng = 360 - brng; // count degrees counter-clockwise - remove to make clockwise

            return brng;
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
    }
}