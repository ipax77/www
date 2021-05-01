
using Microsoft.JSInterop;

namespace www.pwa.Client.Services {
    public class GeoHelper {
        public GeoHelper(string location) {
            Location = location;
        }

        public string Location {get; set;}

        [JSInvokable]
        public string GetLocation() => Location;
    }
}