using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using LocationService.ViewModels;
using LocationService.Messages;
using LocationService.Models;
using LocationService.Utils;
using CommunityToolkit.Mvvm.Messaging;

namespace LocationService.Tests;

internal class FakePreferencesService : IPreferencesService
{
    private readonly Dictionary<string, object> store = new Dictionary<string, object>();

    public void Set<T>(string key, T value) => store[key] = value!;

    public T Get<T>(string key, T defaultValue) => store.TryGetValue(key, out var v) ? (T)v : defaultValue;
}

public class MainPageViewModelTests
{
    private FakePreferencesService prefs = null!;

    [SetUp]
    public void Setup()
    {
        prefs = new FakePreferencesService();
    }

    [Test]
    public void HandleLocationUpdate_UpdatesCoordinatesAndMessage()
    {
        var vm = new MainPageViewModel(prefs);
        var location = new LocationModel(12.3, 45.6);

        WeakReferenceMessenger.Default.Send(new LocationUpdate(location));

        Assert.That(vm.Latitude, Is.EqualTo(12.3));
        Assert.That(vm.Longitude, Is.EqualTo(45.6));
        Assert.That(vm.UserMessage, Is.EqualTo("Location Updated"));
    }

    [Test]
    public void HandleServiceMessage_StartSetsMessage()
    {
        var vm = new MainPageViewModel(prefs);

        WeakReferenceMessenger.Default.Send(new ServiceMessage(ActionsEnum.START));

        Assert.That(vm.UserMessage, Is.EqualTo("Location Service has been started!"));
    }

    [Test]
    public void HandleServiceMessage_StopSetsMessage()
    {
        var vm = new MainPageViewModel(prefs);

        WeakReferenceMessenger.Default.Send(new ServiceMessage(ActionsEnum.STOP));

        Assert.That(vm.UserMessage, Is.EqualTo("Location Service has been stopped!"));
    }

    [Test]
    public void HandleErrorMessage_SetsUserMessage()
    {
        var vm = new MainPageViewModel(prefs);

        WeakReferenceMessenger.Default.Send(new LocationErrorMessage("error message") { Exception = new System.Exception("ex") });

        Assert.That(vm.UserMessage, Is.EqualTo("error message"));
    }

    [Test]
    public void OnStopClick_SetsStateAndPreferences()
    {
        var vm = new MainPageViewModel(prefs);

        prefs.Set(Constants.SERVICE_STATUS_KEY, true);

        vm.OnStopClick();

        Assert.That(vm.StartEnabled, Is.True);
        Assert.That(vm.StopEnabled, Is.False);
        Assert.That(vm.UserMessage, Is.EqualTo("Location Service has been stopped!"));
        Assert.That(prefs.Get(Constants.SERVICE_STATUS_KEY, false), Is.False);
    }

    [Test]
    public async Task ValidateStatus_WhenPreferenceFalse_DoesNotStart()
    {
        var vm = new MainPageViewModel(prefs);

        await vm.ValidateStatus();

        Assert.That(vm.StartEnabled, Is.True);
        Assert.That(vm.StopEnabled, Is.False);
    }
}
