using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorldWideWalk.Messages;
using WorldWideWalk.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorldWideWalk.Services
{
    public class Location
    {
        readonly bool stopping = false;
        public static List<LocationUpdatedEventArgs> Locations = new List<LocationUpdatedEventArgs>();
        public DateTime StartTime;

        public Location()
        {
        }

        public async Task Run(CancellationToken token)
        {
            Locations = new List<LocationUpdatedEventArgs>();
            StartTime = DateTime.UtcNow;
            int i = 0;

            await Task.Run(async () =>
            {
                while (!stopping)
                {
                    if ((StartTime - DateTime.UtcNow).TotalHours > 2)
                    {
                        var message = new StopServiceMessage()
                        {
                            Message = "Der Lauf wurde nach 2h automatisch beendet."
                        };
                        MessagingCenter.Send(message, "ServiceStopped");
                    }

                    token.ThrowIfCancellationRequested();

                    try
                    {
                        await Task.Delay(2000);

                        var request = new GeolocationRequest(GeolocationAccuracy.High);
                        var location = await Geolocation.GetLocationAsync(request);
                        if (location != null)
                        {
                            Locations.Add(new LocationUpdatedEventArgs(
                                location.Latitude,
                                location.Longitude,
                                location.Accuracy,
                                location.Timestamp
                            ));

                            if (i % 10 == 0)
                            {
                                var message = new LocationMessage
                                {
                                    Latitude = location.Latitude,
                                    Longitude = location.Longitude
                                };

                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    MessagingCenter.Send<LocationMessage>(message, "Location");
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var errormessage = new LocationErrorMessage()
                            {
                                Error = ex.Message,
                            };
                            if (i == 1)
                            {
                                errormessage.isFatal = true;
                            }
                            MessagingCenter.Send<LocationErrorMessage>(errormessage, "LocationError");
                        });
                    }
                    i++;
                }
                return;
            }, token);
        }
    }
}
