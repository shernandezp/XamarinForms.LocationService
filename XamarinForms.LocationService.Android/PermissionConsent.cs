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

using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;
using XamarinForms.LocationService.Android;

namespace XamarinForms.LocationService.Droid;

public class PermissionConsent : IPermissionConsent
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
        var context = Application.Context;
        var activity = CurrentActivityUtil.GetCurrentActivity();
        if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.PostNotifications) != Permission.Granted)
        {
            ActivityCompat.RequestPermissions(activity, [Manifest.Permission.PostNotifications], 0);
        }
    }
}