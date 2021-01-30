# Background Location Service

XamarinForms.LocationService is an application that refreshes every 2 seconds GPS location. For years I have been working developing mobile apps that require location features; hopefully, the current project will save you some time in regarding service and location management in your Xamarin application for Android and iOS.

  - Location Updates
  - Location Permissions Management
  - Background Processing Management

For the documentation related to the Background Service in Android/iOs, you can refer to this [tutorial](https://github.com/user/repo/blob/branch/other_file.md). That was the basis of how I started to build a solution of periodic location updates.

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

## IOs:
None.

**Feel free to use the code in your project; your suggestions are more than welcome!!**
