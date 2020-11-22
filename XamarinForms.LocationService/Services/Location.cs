using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinForms.LocationService.Messages;

namespace XamarinForms.LocationService.Services
{
    public class Location
    {

		public Location()
		{
		}

		public void setRunningStateLocationService(bool isRunning)
		{
			if (isRunning)
			{
				Application.Current.Properties["locationServiceIsRunning"] = true;
			}
			else
			{
				Application.Current.Properties["locationServiceIsRunning"] = false;
			}
		}
		public bool getRunningStateLocationService()
		{
			bool locationServiceIsRunning;
			if (Application.Current.Properties.ContainsKey("locationServiceIsRunning"))
			{
				locationServiceIsRunning = Convert.ToBoolean(Application.Current.Properties["locationServiceIsRunning"]);
			}
			else
			{
				locationServiceIsRunning = false;
			}
			return locationServiceIsRunning;
		}

		public async Task Run(CancellationToken token)
		{
			await Task.Run(async () => {
				System.Diagnostics.Debug.WriteLine(getRunningStateLocationService());
				while (getRunningStateLocationService())
				{
					token.ThrowIfCancellationRequested();
					try
					{
						await Task.Delay(2000);

						var request = new GeolocationRequest(GeolocationAccuracy.High);
						var location = await Geolocation.GetLocationAsync(request);
						if (location != null)
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
