﻿using System;
using Xamarin.Forms;

namespace WorldWideWalk
{
    public partial class App : Application
    {
        public const string API = "https://www.pax77.org/www/WwwRun";
        public static TimeSpan MaxRunTime = TimeSpan.FromHours(2);
        public static double MaxSpeedInKmH = 40;

        public App()
        {
            InitializeComponent();

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