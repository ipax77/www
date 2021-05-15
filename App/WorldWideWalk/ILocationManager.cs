using System;
using System.Collections.Generic;
using System.Text;
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
