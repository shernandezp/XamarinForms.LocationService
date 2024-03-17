# Background Location Service

XamarinForms.LocationService is an application that refreshes GPS location every n seconds. Over the years, I've been developing mobile apps requiring location features. Hopefully, this project will save you time regarding service and location management in your Xamarin application for Android and iOS.

  - Location Updates
  - Location Permissions Management
  - Background Processing Management

For documentation related to the Background Services in Android/iOS, you can refer to this [tutorial](https://www.youtube.com/watch?v=Z1YzyreS4-o). It served as the basis for how I started to build a solution for periodic location updates.

For migrating from Xamarin.Forms to MAUI, you can follow this [link](https://learn.microsoft.com/en-us/dotnet/maui/migration/?view=net-maui-8.0)

## The application has been migrated to MAUI.

# Components used

  - MAUI
  - CLLocationManager
  
  ![Image](https://raw.githubusercontent.com/shernandezp/XamarinForms.LocationService/master/screenshot.jpeg)

## Android:
   "Be aware that you might need to adjust battery saver settings on some devices manually to allow the application to continue working in the background."