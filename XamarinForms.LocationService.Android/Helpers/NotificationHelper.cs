using XamarinForms.LocationService.Droid.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHelper))]
namespace XamarinForms.LocationService.Droid.Helpers
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using AndroidX.Core.App;

    internal class NotificationHelper : INotification
    {
        private static readonly string foregroundChannelId = "MyForegroundChannelId";
        private static readonly Context context = Application.Context;


        public Notification ReturnNotif()
        {
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.PutExtra("Title", "Message");

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);

            var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle("Your Title")
                .SetContentText("Your Message")
                .SetSmallIcon(Resource.Drawable.location)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High)
                {
                    Importance = NotificationImportance.High
                };
                notificationChannel.EnableLights(true);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetShowBadge(true);
                notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300 });

                if (context.GetSystemService(Context.NotificationService) is NotificationManager notifManager)
                {
                    notifBuilder.SetChannelId(foregroundChannelId);
                    notifManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notifBuilder.Build();
        }
    }
}