using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorldWideWalk
{
    public partial class App : Application
    {
        // Config
        public const string API = "https://www.pax77.org/www/WwwRun";
        public static TimeSpan MaxRunTime = TimeSpan.FromHours(2);
        public const double MaxSpeedInKmH = 40;

        public App()
        {
            InitializeComponent();
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
