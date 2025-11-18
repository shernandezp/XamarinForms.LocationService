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
using LocationService.Messages;
using LocationService.Models;

namespace LocationService.BackgroundServices;

internal sealed class LocationUpdates
{
    bool stopping = false;

    public async Task Run(CancellationToken token)
    {
        while (!stopping)
        {
            stopping = token.IsCancellationRequested;
            try
            {
                await Task.Delay(5000, token);

                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(1));
                var location = await Geolocation.GetLocationAsync(request, token);
                if (location != null)
                {
                    var message = new LocationModel(location.Latitude, location.Longitude);

                    WeakReferenceMessenger.Default.Send(new LocationUpdate(message));
                }
            }
            catch (Exception ex)
            {
                var errormessage = new LocationErrorMessage(ex.Message)
                {
                    Exception = ex
                };
                WeakReferenceMessenger.Default.Send(errormessage);
            }
        }
    }
}
