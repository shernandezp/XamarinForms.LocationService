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
using Android.Content.PM;
using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using LocationService.BackgroundServices;
using LocationService.Messages;
using LocationService.Platforms.Android.Helpers;
using LocationService.Utils;

namespace LocationService.Platforms.Android.Services;

[Service(ForegroundServiceType = ForegroundService.TypeLocation)]
internal class AndroidLocationService : Service
{
    CancellationTokenSource? _cts;
    public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

    public override IBinder OnBind(Intent? intent)
    {
        return null;
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        _cts = new CancellationTokenSource();

        var notificationHelper = new NotificationHelper();
        var notification = notificationHelper.ReturnNotification();
        if (Build.VERSION.SdkInt > BuildVersionCodes.Q)
        {
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification,
                ForegroundService.TypeLocation);
        }
        else
        {
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        Task.Run(async () => {
            try
            {
                var locShared = new LocationUpdates();
                await locShared.Run(_cts.Token);
            }
            catch
            {
                // Service could not start
            }
            finally
            {
                if (_cts.IsCancellationRequested)
                {
                    WeakReferenceMessenger.Default.Send(new ServiceMessage(ActionsEnum.STOP));
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
