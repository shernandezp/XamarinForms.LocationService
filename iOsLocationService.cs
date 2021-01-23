using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Services;

namespace XamarinForms.LocationService.iOS.Services
{
    public class iOsLocationService
    {
        nint _taskId;
        CancellationTokenSource _cts;

        public async Task Start()
        {
            _cts = new CancellationTokenSource();
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("com.company.product.name", OnExpiration);

            try
            {
                var locShared = new Location();
                locShared.setRunningStateLocationService(true); 
                await locShared.Run(_cts.Token);

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

            var time = UIApplication.SharedApplication.BackgroundTimeRemaining;

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        void OnExpiration()
        {
            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }
    }
}