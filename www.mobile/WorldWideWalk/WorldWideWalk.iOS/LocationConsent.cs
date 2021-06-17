using CoreLocation;
using System.Threading.Tasks;
using UIKit;
using WorldWideWalk.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(LocationConsent))]
namespace WorldWideWalk.iOS
{
    public class LocationConsent : ILocationConsent
    {
        readonly CLLocationManager locMgr = new CLLocationManager()
        {
            PausesLocationUpdatesAutomatically = false
        };
        public LocationConsent()
        {
        }
        public async Task<bool> GetLocationConsent()
        {
            //Background Location Permissions
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locMgr.AllowsBackgroundLocationUpdates = true;
            }
            return true;
        }
    }
}