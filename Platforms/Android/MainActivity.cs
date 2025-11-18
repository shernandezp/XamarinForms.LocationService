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
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Provider;
using CommunityToolkit.Mvvm.Messaging;
using LocationService.Platforms.Android.Services;
using LocationService.Messages;
using LocationService.Utils;

namespace LocationService;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    Intent? serviceIntent;
    private const int RequestCode = 5469;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        serviceIntent = new Intent(this, typeof(AndroidLocationService));
        WeakReferenceMessenger.Default.Register<ServiceMessage>(this, HandleServiceMessage);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.M && !Settings.CanDrawOverlays(this))
        {
            var intent = new Intent(Settings.ActionManageOverlayPermission);
            intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        }
    }

    private void HandleServiceMessage(object recipient, ServiceMessage message)
    {
        if (message.Value == ActionsEnum.START)
        {
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
        }
        else
        {
            if (IsServiceRunning(typeof(AndroidLocationService)))
                StopService(serviceIntent);
        }
    }

    public bool IsServiceRunning(Type serviceClass)
    {
        var manager = (ActivityManager)GetSystemService(ActivityService);
        foreach (var service in manager.GetRunningServices(int.MaxValue))
        {
            if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(serviceClass).CanonicalName))
            {
                return true;
            }
        }
        return false;
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode == RequestCode)
        {
            if (Settings.CanDrawOverlays(this))
            {

            }
        }

        base.OnActivityResult(requestCode, resultCode, data);
    }
}
