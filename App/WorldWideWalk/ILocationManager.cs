using System;
using WorldWideWalk.Models;

namespace WorldWideWalk
{
    public interface ILocationManager
    {
        event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        void StartLocationUpdates();
        void StopLocationUpdates();
    }
}
