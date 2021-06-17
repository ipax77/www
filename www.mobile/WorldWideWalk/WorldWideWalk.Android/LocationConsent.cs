using System.Threading.Tasks;
using WorldWideWalk.Droid;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(LocationConsent))]
namespace WorldWideWalk.Droid
{
    public class LocationConsent : ILocationConsent
    {
        public async Task<bool> GetLocationConsent()
        {
            var status = await Permissions.RequestAsync<Permissions.LocationAlways>();
            if (status == PermissionStatus.Granted)
                return true;
            else
                return false;
        }
    }
}