using Microsoft.Maui.Storage;

namespace LocationService.Utils;

internal class PreferencesService : IPreferencesService
{
    public void Set<T>(string key, T value) => Preferences.Default.Set(key, value);

    public T Get<T>(string key, T defaultValue) => Preferences.Default.Get(key, defaultValue);
}
