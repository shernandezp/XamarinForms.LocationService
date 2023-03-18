namespace XamarinForms.LocationService.Droid
{
    using Android.App;
    using Android.Content.PM;
    using Android.Runtime;
    using Android.OS;
    using Android.Content;
    using Xamarin.Forms;
    using XamarinForms.LocationService.Droid.Services;
    using XamarinForms.LocationService.Messages;

    [Activity(Label = "XamarinForms.LocationService", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        Intent serviceIntent;
        private const int RequestCode = 5469;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            serviceIntent = new Intent(this, typeof(AndroidLocationService));
            SetServiceMethods();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M && !Android.Provider.Settings.CanDrawOverlays(this))
            {
                var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission);
                intent.SetFlags(ActivityFlags.NewTask);
                this.StartActivity(intent);
            }

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void SetServiceMethods()
        {
            MessagingCenter.Subscribe<StartServiceMessage>(this, "ServiceStarted", message => {
                if (!IsServiceRunning(typeof(AndroidLocationService)))
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        StartForegroundService(serviceIntent);
                    }
                    else
                    {
                        StartService(serviceIntent);
                    }
                }
            });

            MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message => {
                if (IsServiceRunning(typeof(AndroidLocationService)))
                    StopService(serviceIntent);
            });
        }

        public bool IsServiceRunning(System.Type serviceClass)
        {
            ActivityManager activityManager = (ActivityManager)GetSystemService(Context.ActivityService);
            var runningAppProcesses = activityManager.RunningAppProcesses;
            if (runningAppProcesses != null)
            {
                foreach (var processInfo in runningAppProcesses)
                {
                    if (processInfo.Importance == Importance.Foreground)
                    {
                        foreach (string activeProcess in processInfo.PkgList)
                        {
                            if (activeProcess.Equals(serviceClass.Name))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == RequestCode)
            {
                if (Android.Provider.Settings.CanDrawOverlays(this))
                {
                    
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}