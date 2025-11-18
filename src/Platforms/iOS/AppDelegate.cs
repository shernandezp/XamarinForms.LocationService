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

using CommunityToolkit.Mvvm.Messaging;
using CoreLocation;
using Foundation;
using LocationService.Messages;
using LocationService.Platforms.iOS;
using LocationService.Platforms.iOS.Services;
using LocationService.Utils;
using UIKit;

namespace LocationService;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private nint backgroundTaskId;
    private iOsLocationService? locationService;
    private readonly CLLocationManager locMgr = new();

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication app, NSDictionary? options)
    {
        locationService = new iOsLocationService();
        WeakReferenceMessenger.Default.Register<ServiceMessage>(this, HandleServiceMessage);
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

        DependencyService.Register<IPermissionConsent, PermissionConsent>();

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

    private async void HandleServiceMessage(object recipient, ServiceMessage message)
    {
        if (message.Value == ActionsEnum.START)
        {
            if (locationService != null && !locationService.isStarted)
                await locationService.Start();
        }
        else
        {
            if (locationService != null && locationService.isStarted)
                locationService.Stop();
        }
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
