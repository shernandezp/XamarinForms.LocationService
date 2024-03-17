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

namespace XamarinForms.LocationService.ViewModels;

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Utils;

public class MainPageViewModel : BaseViewModel
{
    #region vars
    private double latitude;
    private double longitude;
    public string userMessage;
    public bool startEnabled;
    public bool stopEnabled;
    #endregion vars

    #region properties
    public double Latitude
    {
        get => latitude;
        set => SetProperty(ref latitude, value);
    }
    public double Longitude
    {
        get => longitude;
        set => SetProperty(ref longitude, value);
    }
    public string UserMessage
    {
        get => userMessage;
        set => SetProperty(ref userMessage, value);
    }
    public bool StartEnabled
    {
        get => startEnabled;
        set => SetProperty(ref startEnabled, value);
    }
    public bool StopEnabled
    {
        get => stopEnabled;
        set => SetProperty(ref stopEnabled, value);
    }
    #endregion properties

    #region commands
    public Command StartCommand { get; }
    public Command EndCommand { get; }
    #endregion commands

    public MainPageViewModel()
    {
        StartCommand = new Command(async () => await OnStartClick());
        EndCommand = new Command(async() => await OnStopClick());
        WeakReferenceMessenger.Default.Register<LocationUpdate>(this, HandleLocationUpdate);
        WeakReferenceMessenger.Default.Register<ServiceMessage>(this, HandleServiceMessage);
        WeakReferenceMessenger.Default.Register<LocationErrorMessage>(this, HandleErrorMessage);
        StartEnabled = true;
        StopEnabled = false;
    }

    public async Task OnStartClick()
    {
        await Start();
    }

    public async Task OnStopClick()
    {
        WeakReferenceMessenger.Default.Send(new ServiceMessage(ActionsEnum.STOP));
        UserMessage = "Location Service has been stopped!";
        await SecureStorage.SetAsync(Constants.SERVICE_STATUS_KEY, "0");
        StartEnabled = true;
        StopEnabled = false;
    }

    public async Task ValidateStatus() 
    {
        var status = await SecureStorage.GetAsync(Constants.SERVICE_STATUS_KEY);
        if (status != null && status.Equals("1")) 
        {
            await Start();
        }
    }

    async Task Start() 
    {
        await PermissionHelper.CheckLocationPermissions();
        PermissionHelper.CheckNotificationPermissions();
        WeakReferenceMessenger.Default.Send(new ServiceMessage(ActionsEnum.START));
        await SecureStorage.SetAsync(Constants.SERVICE_STATUS_KEY, "1");
        StartEnabled = false;
        StopEnabled = true;
    }

    private void HandleServiceMessage(object recipient, ServiceMessage message)
        => UserMessage = message.Value == ActionsEnum.STOP ? "Location Service has been stopped!" : "Location Service has been started!";

    private void HandleErrorMessage(object recipient, LocationErrorMessage message)
        => UserMessage = message.Value;

    private void HandleLocationUpdate(object recipient, LocationUpdate location)
    {
        Latitude = location.Value.Latitude;
        Longitude = location.Value.Longitude;
        UserMessage = "Location Updated";
    }
}
