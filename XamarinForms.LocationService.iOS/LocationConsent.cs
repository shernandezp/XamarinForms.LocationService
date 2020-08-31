using CoreLocation;
using System.Threading.Tasks;
using UIKit;
using XamarinForms.LocationService.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(LocationConsent))]
namespace XamarinForms.LocationService.iOS
{
    public class LocationConsent : ILocationConsent
    {
        public static LocationManager Manager { get; set; }
        public LocationConsent()
        {
            Manager = new LocationManager();
            Manager.StartLocationUpdates();
        }
        public async Task GetLocationConsent()
        {
            var manager = new CLLocationManager();
            manager.AuthorizationChanged += (sender, args) => {
                //Console.WriteLine("Authorization changed to: {0}", args.Status);
            };
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                manager.RequestWhenInUseAuthorization();
            }
        }
    }
}