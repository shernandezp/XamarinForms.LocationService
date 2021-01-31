using System.Threading.Tasks;
using Xamarin.Essentials;
using XamarinForms.LocationService.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(LocationConsent))]
namespace XamarinForms.LocationService.Droid
{
    public class LocationConsent : ILocationConsent
    {
        public async Task GetLocationConsent()
        {
            await Permissions.RequestAsync<Permissions.LocationAlways>();
        }
    }
}