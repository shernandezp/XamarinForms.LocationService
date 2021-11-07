# Background Location Service

XamarinForms.LocationService is an application that refreshes every n seconds GPS location. For years I have been working developing mobile apps that require location features; hopefully, the current project will save you some time in regarding service and location management in your Xamarin application for Android and iOS.

  - Location Updates
  - Location Permissions Management
  - Background Processing Management

For the documentation related to the Background Service in Android/iOs, you can refer to this [tutorial](https://www.youtube.com/watch?v=Z1YzyreS4-o). That was the basis of how I started to build a solution of periodic location updates.

# Components used

  - Xamarin.Essentials
  - MessagingCenter
  - CLLocationManager
  
  ![Image](https://raw.githubusercontent.com/shernandezp/XamarinForms.LocationService/master/screenshot.jpeg)

I have updated the application to Xamarin 5. It seems everything is working well until now.
So far the upgrade only required 2 things:

## Android:
- To add the "ACCESS_BACKGROUD_LOCATION" permission.
- To add using of AndroidX.Core.App for the NotificationHelper (It's required for the NotificationCompat.Builder).

   "Be aware that you might need to adjust battery saver settings on some devices manually to allow the application to keep working in the background."

## IOs:
None.

## New features:
- Android: As long as the user opens the application, and starts the service only once, it will be always up, even after rebooting the device.
- iOS: As long as the application is open and the user has started the service, it will be up. If the user closes the applications or reboots the device, the service will stop. I haven't found a way to keep the service always up. (Other Native applications can keep a service always up, for example [here](https://github.com/traccar/traccar-client-ios) ).

**Feel free to use the code in your project; your suggestions are more than welcome!!**
