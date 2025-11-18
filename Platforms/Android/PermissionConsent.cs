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

using Android;
using AndroidX.Core.App;

namespace LocationService.Platforms.Android;

internal class PermissionConsent : IPermissionConsent
{
    public async Task GetLocationConsent()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown)
            {
                await Permissions.RequestAsync<Permissions.LocationAlways>();
            }
        }
    }
    public void GetNotificationsConsent()
    {
        var activity = Platform.CurrentActivity;
        if (activity != null)
        {
            ActivityCompat.RequestPermissions(activity, [Manifest.Permission.PostNotifications], 1001);
        }
        else
        {
            // Handle the case where activity is null (e.g., log or show error)
        }
    }
}