using System.Threading.Tasks;
using Xamarin.Essentials;
using WorldWideWalk.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(LocationConsent))]
namespace WorldWideWalk.Droid
{
    public class LocationConsent : ILocationConsent
    {
        public async Task GetLocationConsent()
        {
            await Permissions.RequestAsync<Permissions.LocationAlways>();
        }
    }
}