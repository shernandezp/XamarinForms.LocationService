using Android.App;
using Android.Content;
using System.Threading.Tasks;
using Android.OS;
using System.Threading;
using Xamarin.Forms;
using XamarinForms.LocationService.Services;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Droid.Helpers;

namespace XamarinForms.LocationService.Droid.Services
{
    [Service]
    public class AndroidLocationService : Service
    {
		private Location locShared;
		CancellationTokenSource _cts;
		public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			_cts = new CancellationTokenSource();

			Notification notif = DependencyService.Get<INotification>().ReturnNotif();
			StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notif);

			Task.Run(() => {
				try
				{
					var locShared = new Location();
					locShared.Run(_cts.Token).Wait();
				}
				catch (OperationCanceledException)
				{
				}
				finally
				{
					if (_cts.IsCancellationRequested)
					{
						var message = new StopServiceMessage();
						Device.BeginInvokeOnMainThread(
							() => MessagingCenter.Send(message, "ServiceStopped")
						);
					}
				}
			}, _cts.Token);

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
			if (_cts != null)
			{
				_cts.Token.ThrowIfCancellationRequested();
				_cts.Cancel();
			}
			base.OnDestroy();
		}
	}
}