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

namespace LocationService.Utils;

internal static class PermissionHelper
{
    public static async Task<bool> CheckLocationPermissions()
    {
        var windows = Application.Current?.Windows;
        var page = windows != null && windows.Count > 0 ? windows[0].Page : null;
        if (page == null)
            return false;

        var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        if (status == PermissionStatus.Denied)
        {
            await page.DisplayAlertAsync("Info", "This app collects location data even when the app is closed or not in use.", "OK");
            var locationConsent = DependencyService.Get<IPermissionConsent>();
            await locationConsent.GetLocationConsent();
        }

        status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        return (status == PermissionStatus.Granted);
    }

    public static void CheckNotificationPermissions()
    {

        var locationConsent = DependencyService.Get<IPermissionConsent>();
        locationConsent.GetNotificationsConsent();
    }
}