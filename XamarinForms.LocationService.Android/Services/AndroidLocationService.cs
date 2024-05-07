// Copyright (c) 2024 Sergio Hernandez. All rights reserved.
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

using Android.OS;
using Android.App;
using Android.Content;

namespace XamarinForms.LocationService.Droid.Services
{
    using System.Threading.Tasks;
    
    using System.Threading;
    using XamarinForms.LocationService.Services;
    using XamarinForms.LocationService.Messages;
    using XamarinForms.LocationService.Droid.Helpers;
    using Microsoft.Maui.Controls;
    using CommunityToolkit.Mvvm.Messaging;
    using XamarinForms.LocationService.Utils;
    using global::Android.Content.PM;

    [Service(ForegroundServiceType = ForegroundService.TypeDataSync)]
    public class AndroidLocationService : Service
    {
		CancellationTokenSource _cts;
		public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			_cts = new CancellationTokenSource();

			var notification = DependencyService.Get<INotification>().ReturnNotification();
            if (Build.VERSION.SdkInt > BuildVersionCodes.Q)
            {
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification,
                ForegroundService.TypeDataSync);
            }
            else
            {
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }

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
}