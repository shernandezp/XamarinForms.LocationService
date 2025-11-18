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

using CoreLocation;
using UIKit;

namespace LocationService.Platforms.iOS;

internal class PermissionConsent : IPermissionConsent
{
    public static LocationManager? Manager { get; set; }
    public PermissionConsent()
    {
        Manager = new LocationManager();
        Manager.StartLocationUpdates();
    }
    public async Task GetLocationConsent()
    {
        var manager = new CLLocationManager();
        manager.AuthorizationChanged += (sender, args) => {
            //Console.WriteLine("Authorization changed to: {0}", args.Status);
        };
        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
        {
            manager.RequestAlwaysAuthorization();
        }
    }

    public void GetNotificationsConsent() { }
}