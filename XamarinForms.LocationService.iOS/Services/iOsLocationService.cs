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
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Services;
using CommunityToolkit.Mvvm.Messaging;
using XamarinForms.LocationService.Utils;

namespace XamarinForms.LocationService.iOS.Services;

public class iOsLocationService
{
    nint _taskId;
    CancellationTokenSource _cts;
    public bool isStarted = false;

    public async Task Start()
    {
        _cts = new CancellationTokenSource();
        _taskId = UIApplication.SharedApplication.BeginBackgroundTask("com.company.product.name", OnExpiration);

        try
        {
            var locShared = new Location();
            isStarted = true;
            await locShared.Run(_cts.Token);

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

        var time = UIApplication.SharedApplication.BackgroundTimeRemaining;

        UIApplication.SharedApplication.EndBackgroundTask(_taskId);
    }

    public void Stop()
    {
        isStarted = false;
        _cts.Cancel();
    }

    void OnExpiration()
    {
        UIApplication.SharedApplication.EndBackgroundTask(_taskId);
    }
}
