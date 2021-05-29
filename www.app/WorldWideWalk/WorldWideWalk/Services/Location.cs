using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using WorldWideWalk.Messages;
using System.Collections.Generic;
using WorldWideWalk.Models;

namespace WorldWideWalk.Services
{
    public class Location
    {
		readonly bool stopping = false;
		public static List<LocationUpdatedEventArgs> Locations = new List<LocationUpdatedEventArgs>();

		public Location()
		{
		}

		public async Task Run(CancellationToken token)
		{
			Locations = new List<LocationUpdatedEventArgs>();

			await Task.Run(async () => {
				while (!stopping)
				{
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
					catch (Exception ex)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							var errormessage = new LocationErrorMessage();
							MessagingCenter.Send<LocationErrorMessage>(errormessage, "LocationError");
						});
					}
				}
				return;
			}, token);
		}
	}
}
