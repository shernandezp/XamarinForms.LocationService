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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Utils;

public partial class MainPageViewModel : BaseViewModel
{
    #region properties

    [ObservableProperty]
    private double latitude;
    [ObservableProperty]
    private double longitude;
    [ObservableProperty]
    public string userMessage;
    [ObservableProperty]
    public bool startEnabled;
    [ObservableProperty]
    public bool stopEnabled;

    #endregion properties

    #region events
    [RelayCommand]
    private async Task StartEventAsync() => await OnStartClick();
    [RelayCommand]
    private async Task StopEventAsync() => await OnStopClick();
    #endregion events

    public MainPageViewModel()
    {
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
