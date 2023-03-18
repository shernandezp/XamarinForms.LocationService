namespace XamarinForms.LocationService.iOS
{
    using CoreLocation;
    using Foundation;
    using System;
    using UIKit;
    using Xamarin.Forms;
    using XamarinForms.LocationService.iOS.Services;
    using XamarinForms.LocationService.Messages;

    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private nint backgroundTaskId;
        private iOsLocationService locationService;
        private readonly CLLocationManager locMgr = new CLLocationManager();
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            locationService = new iOsLocationService();
            SetServiceMethods();
            Forms.Init();
            LoadApplication(new App());
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            //Background Location Permissions
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locMgr.AllowsBackgroundLocationUpdates = true;
            }

            return base.FinishedLaunching(app, options);
        }

        public override void OnResignActivation(UIApplication uiApplication)
        {
            base.OnResignActivation(uiApplication);

            // Request a background task to keep the app running in the background.
            backgroundTaskId = UIApplication.SharedApplication.BeginBackgroundTask(() => {
                // Perform cleanup operations when the background task is about to expire.
                UIApplication.SharedApplication.EndBackgroundTask(backgroundTaskId);
                backgroundTaskId = nint.MinValue;
            });
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            base.DidEnterBackground(uiApplication);

            // Continue executing the background task.
            if (backgroundTaskId != nint.MinValue)
            {
                // Your app is currently running a background task.
                // Keep the app running in the background for as long as possible.
                UIApplication.SharedApplication.EndBackgroundTask(backgroundTaskId);
                backgroundTaskId = UIApplication.SharedApplication.BeginBackgroundTask(() => {
                    // Perform cleanup operations when the background task is about to expire.
                    UIApplication.SharedApplication.EndBackgroundTask(backgroundTaskId);
                    backgroundTaskId = nint.MinValue;
                });
            }
        }

        void SetServiceMethods()
        {
            MessagingCenter.Subscribe<StartServiceMessage>(this, "ServiceStarted", async message => {
                if (!locationService.isStarted)
                    await locationService.Start();
            });

            MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message => {
                if (locationService.isStarted)
                    locationService.Stop();
            });
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                completionHandler(UIBackgroundFetchResult.NewData);
            }
            catch (Exception)
            {
                completionHandler(UIBackgroundFetchResult.NoData);
            }
        }
    }
}
