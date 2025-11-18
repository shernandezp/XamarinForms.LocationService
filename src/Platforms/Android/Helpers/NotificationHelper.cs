// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace LocationService.Platforms.Android.Helpers
{
    internal class NotificationHelper
    {
        private static readonly string foregroundChannelId = "MyForegroundChannelId";
        private static readonly Context context = global::Android.App.Application.Context;
        private static bool _channelCreated = false;
        private static readonly Lock _lock = new();

        public Notification ReturnNotification()
        {
            // Ensure channel is created only once
            EnsureChannelCreated();

            // Building intent
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);

            var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle("Your Title")
                .SetContentText("Your Message")
                .SetSmallIcon(Resource.Drawable.location)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent);

            return notifBuilder.Build();
        }

        private static void EnsureChannelCreated()
        {
            // Only create channel once on Android O and above
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O && !_channelCreated)
            {
                lock (_lock)
                {
                    if (!_channelCreated)
                    {
                        var notificationChannel = new NotificationChannel(
                            foregroundChannelId,
                            "Position synchronization",
                            NotificationImportance.Low)
                        {
                            Description = "Background position synchronization notifications"
                        };

                        notificationChannel.EnableLights(true);
                        notificationChannel.EnableVibration(true);
                        notificationChannel.SetShowBadge(true);
                        notificationChannel.SetVibrationPattern([100, 200, 300]);

                        if (context.GetSystemService(Context.NotificationService) is NotificationManager notifierManager)
                        {
                            notifierManager.CreateNotificationChannel(notificationChannel);
                        }

                        _channelCreated = true;
                    }
                }
            }
        }
    }
}
