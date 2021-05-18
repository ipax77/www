﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorldWideWalk
{
    public partial class App : Application
    {
        // Config
        // public const string API = "https://www.pax77.org/www/api/";
        public const string API = "https://127.0.0.1:5001/WwwRun/";
        public static TimeSpan MaxRunTime = TimeSpan.FromHours(2);
        public const double MaxSpeedInKmH = 40;

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