using Android.Content;
using WorldWideWalk.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidServiceHelper))]
namespace WorldWideWalk.Droid
{
    internal class AndroidServiceHelper : IAndroidService
    {
        private static Context context = global::Android.App.Application.Context;

        public void StartService()
        {
            var intent = new Intent(context, typeof(DataSource));

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                context.StartForegroundService(intent);
            }
            else
            {
                context.StartService(intent);
            }
        }

        public void StopService()
        {
            var intent = new Intent(context, typeof(DataSource));
            context.StopService(intent);
        }
    }
}