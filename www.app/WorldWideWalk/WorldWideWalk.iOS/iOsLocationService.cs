﻿using CoreLocation;
using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using WorldWideWalk.Messages;
using WorldWideWalk.Services;
using Xamarin.Forms;

namespace WorldWideWalk.iOS.Services
{
    public class iOsLocationService
    {
        nint _taskId;
        CancellationTokenSource _cts;
        public bool isStarted = false;
        readonly CLLocationManager locMgr = new CLLocationManager();

        public async Task Start()
        {
            _cts = new CancellationTokenSource();
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("com.mycompany.worldwidewalk", OnExpiration);

            //Background Location Permissions
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locMgr.AllowsBackgroundLocationUpdates = true;
            }

            try
            {
                var locShared = new Location();
                // locShared.setRunningStateLocationService(true);
                await locShared.Run(_cts.Token);
                isStarted = true;

            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (_cts.IsCancellationRequested)
                {
                    var message = new StopServiceMessage();
                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(message, "ServiceStopped")
                    );
                    isStarted = false;
                }
            }

            var time = UIApplication.SharedApplication.BackgroundTimeRemaining;

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }

        public void Stop()
        {
            _cts.Cancel();
            isStarted = false;
        }

        void OnExpiration()
        {
            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }
    }
}