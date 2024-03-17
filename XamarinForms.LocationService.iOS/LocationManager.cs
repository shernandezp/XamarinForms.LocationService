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

using System;
using CoreLocation;
using UIKit;

namespace XamarinForms.LocationService.iOS;

public class LocationManager
{
    protected CLLocationManager locMgr;
    public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

    public LocationManager()
    {
        this.locMgr = new CLLocationManager
        {
            PausesLocationUpdatesAutomatically = false
        };

        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
        {
            locMgr.RequestAlwaysAuthorization();
        }

        if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
        {
            locMgr.AllowsBackgroundLocationUpdates = true;
        }
    }

    public CLLocationManager LocMgr
    {
        get { return this.locMgr; }
    }

    public void StartLocationUpdates()
    {
        if (CLLocationManager.LocationServicesEnabled)
        {
            LocMgr.DesiredAccuracy = 1;
            LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
            {
                LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
            };
            LocMgr.StartUpdatingLocation();
        }
    }
}

public class LocationUpdatedEventArgs(CLLocation location) : EventArgs
{
    public CLLocation Location
    {
        get { return location; }
    }
}