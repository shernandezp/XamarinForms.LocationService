# Background Location Service

![Image](https://raw.githubusercontent.com/shernandezp/XamarinForms.LocationService/master/screenshot.jpeg)

LocationService is a .NET MAUI sample app (targeting .NET 10) that demonstrates how to obtain periodic GPS location updates and keep delivering them while the app runs in the foreground and — with platform support and the proper permissions — in the background.

The sample was originally developed to collect and broadcast location updates for mobile apps and includes helpers for permission management, a simple background loop that fetches location periodically, and lightweight messaging to deliver updates to the app UI.

## Key capabilities

- Periodic location updates (configurable interval)
- Permission consent helpers for iOS
- Background processing pattern for polling GPS and sending updates
- Small, testable core with platform-specific implementations when required

## Quick architecture summary

- `src/BackgroundServices/LocationUpdates.cs` — the periodic worker that requests the device location and publishes updates via `WeakReferenceMessenger` (`LocationUpdate` and `LocationErrorMessage`). The default delay is currently 5000 ms and the `GeolocationRequest` uses `GeolocationAccuracy.High` with a 1s timeout; adjust these values to change frequency/accuracy.
- `src/Platforms/iOS/LocationManager.cs` and `src/Platforms/iOS/PermissionConsent.cs` — iOS-specific location manager wrapper and a permission-consent helper that requests "always" authorization and enables `AllowsBackgroundLocationUpdates` when available.
- `src/Platforms/Android/PermissionConsent.cs` — the Android counterpart that is responsible for requesting runtime location permissions and handling consent flows on Android devices.
- `src/Platforms/Android/Services/AndroidLocationService.cs` and `src/Platforms/iOS/Services/iOsLocationService.cs` — platform-specific background service implementations. These files contain the OS integration required to run the `LocationUpdates` worker in the background (for example, Android foreground service / worker integration and the iOS `BeginBackgroundTask` wrapper used in `iOsLocationService`).
- Messaging models in `src/Messages/` (for errors and updates) and `src/Models/LocationModel.cs` hold simple location payloads.

## Supported platforms

- iOS (uses CoreLocation/CLLocationManager; requests Always authorization when appropriate)
- Android (via .NET MAUI Geolocation APIs; requires proper manifest permissions and battery-optimisation tweaks on some devices)
- Mac Catalyst and Windows are included in the project multi-target but platform background semantics differ — consult platform docs.

## Important platform and permission notes

### iOS
- This sample requests Always location access in `PermissionConsent` and sets `AllowsBackgroundLocationUpdates` in `LocationManager` when the OS supports it.
- Update your `Info.plist` with the appropriate usage descriptions (user-facing text):
  - `NSLocationWhenInUseUsageDescription`
  - `NSLocationAlwaysAndWhenInUseUsageDescription`
  - Optionally `NSLocationAlwaysUsageDescription` (older iOS versions)
- Add the `location` background mode in `UIBackgroundModes` if your app needs to continue collecting locations while suspended.

### Android
- Add runtime permissions in `AndroidManifest.xml` and request them at runtime for Android 6+:
  - `ACCESS_FINE_LOCATION`
  - `ACCESS_COARSE_LOCATION`
  - `ACCESS_BACKGROUND_LOCATION` (required to access location while the app is in the background on modern Android versions)
- Some manufacturers aggressively stop background work to save battery — you may need to instruct users to exclude the app from battery optimizations or whitelist it.
- Consider using a foreground service (with a notification) if you need long-running background location updates on Android.

## Configuration and tuning

- Polling frequency: edit `Task.Delay(5000, token)` in `src/BackgroundServices/LocationUpdates.cs` to change how often the sample attempts to fetch location.
- Accuracy/timeout: tune the `GeolocationRequest` parameters in the same file to balance accuracy and battery usage.
- Preferences: the project contains a small preferences service (`src/Utils/PreferencesService.cs`) and a `SERVICE_STATUS_KEY` constant used to persist state.

## How to build and run

### Prerequisites
- Visual Studio with .NET MAUI workload installed (support for .NET 10 in the product matching the SDK target)
- Or install .NET SDK and MAUI workloads: `dotnet workload install maui` (use a terminal that supports your platform)

### Build and run
- Open the solution in Visual Studio and deploy to a simulator or device for the target platform.
- From the command line you can build or test the projects:
  - `dotnet build` — builds all projects
  - `dotnet test tests/LocationService.Tests` — run the test suite

> Notes: Some platforms (iOS/tvOS/macCatalyst/Android) require deployment to a device (not all simulators provide background behaviors or accurate hardware location). For full background behavior test on a real device.

## Testing

The solution includes a tests project at `tests/LocationService.Tests`. Execute `dotnet test` against that project to validate the test suite. Unit tests should be kept platform-agnostic; platform-specific code can be integration-tested on devices.

## Contributing

Contributions, bug reports and suggestions are welcome. If you plan to add features, try to keep platform-specific code isolated under `src/Platforms/*` and common logic under `src/` so unit tests can run without device dependencies.

## License

This repository includes Apache-2.0 license headers in source files. Confirm and add a repository-level `LICENSE` file if you intend to publish.

## References and further reading

- .NET MAUI documentation: https://learn.microsoft.com/dotnet/maui/
- Apple Core Location guide: https://developer.apple.com/documentation/corelocation
- Android location permissions and background location: https://developer.android.com/training/location
