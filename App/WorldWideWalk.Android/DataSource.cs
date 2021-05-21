using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;


namespace WorldWideWalk.Droid
{
    [Service(Name = "com.companyname.worldwidewalk.DataSource")]
    public class DataSource : Service
    {
        private bool isRunning = false;
        private GeoService geoService;
        static readonly string TAG = typeof(DataSource).FullName;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public const int ServiceRunningNotifID = 9077;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (isRunning)
                return StartCommandResult.Sticky;
            isRunning = true;

            //Notification notif = DependencyService.Get<INotification>().ReturnNotif();
            //StartForeground(ServiceRunningNotifID, notif);

            NotificationHelper helper = new NotificationHelper();
            var notif = helper.ReturnNotif();
            StartForeground(ServiceRunningNotifID, notif);

            Log.Debug(TAG, "Start");
            geoService = new GeoService();
            // geoService.StartRun().GetAwaiter().GetResult();
            geoService.StartRun();

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            Log.Debug(TAG, "Destroy");
            geoService.StopRun();
            base.OnDestroy();
        }

        public override bool StopService(Intent name)
        {
            Log.Debug(TAG, "Stop");
            isRunning = false;
            geoService.StopRun();
            return base.StopService(name);
        }


    }
}