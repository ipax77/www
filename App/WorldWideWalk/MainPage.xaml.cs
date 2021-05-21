using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldWideWalk.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorldWideWalk
{
    public partial class MainPage : ContentPage, IDisposable
    {
        private ILocationManager locationManager;
        private IRestService restService;
        private Run Run = new Run();
        private Run DebugRun;
        private WalkAppModel Walk;
        private EntityRunFormData RunData;
        private ICollection<string> validationMessages = new List<string>();
        public string ValidationMessage { get; set; }
        private string walkGuid = "7A40C465-BDC8-4373-B6BE-6E49C10D5ECA";
        private string walkUri = "https://www.pax77.org/www";

        public MainPage()
        {
            InitializeComponent();
            PasswordEntry.Completed += PasswordEntry_Completed;
            PasswordEntry.TextChanged += PasswordEntry_TextChanged;
            PseudonymEntry.Completed += PseudonymEntry_Completed;
            PseudonymEntry.TextChanged += PseudonymEntry_TextChanged;
            restService = new RestService();
            GetWalk();
        }



        private async void OpenLink(object sender, EventArgs e)
        {
            try
            {
                await Browser.OpenAsync(walkUri, BrowserLaunchMode.SystemPreferred);
            }
            catch { }
        }

        private async void Debug()
        {
            // DebugRun = await restService.GetDebugData();
        }

        private async void GetWalk()
        {
            Walk = await restService.GetWalk(walkGuid);
            if (String.IsNullOrEmpty(Walk.Name))
                OfflineLayout.IsVisible = true;
            SetCurrentText();
            // Debug();
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }

        private void StartRun(object sender, EventArgs e)
        {
            SubmitLayout.IsVisible = false;
            MapLayout.IsVisible = false;
            FeedbackLayout.IsVisible = false;
            OfflineLayout.IsVisible = false;
            grMap.Children.Clear();
            LbTime.Text = "";
            LbDistance.Text = "";
            BtnStart.IsEnabled = false;
            BtnStart.Clicked -= StartRun;
            BtnStop.IsEnabled = true;
            BtnStop.Clicked += StopRun;
            Run = new Run();
            Run.StartTime = DateTime.UtcNow;
            locationManager = DependencyService.Get<ILocationManager>();
            locationManager.StartLocationUpdates();
            locationManager.LocationUpdated += LocationManager_LocationUpdated;
        }

        private async void StopRun(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
            BtnStart.IsEnabled = true;
            BtnStart.Clicked += StartRun;
            BtnStop.IsEnabled = false;
            BtnStop.Clicked -= StopRun;
            locationManager.StopLocationUpdates();
            locationManager.LocationUpdated -= LocationManager_LocationUpdated;
            Run.StopTime = DateTime.UtcNow;

            Run.SetRunInfo();

            if (sender != null)
            {
                LbTime.Text = $"Laufzeit: {(Run.StopTime - Run.StartTime).ToString(@"hh\:mm\:ss")}";
            }
            LbDistance.Text = "Zurückgelegt: " + Math.Round(Run.Distance, 2).ToString() + " m";

            bool success = await InitRunData();
            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;
        }

        async void Submit_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
            if (String.IsNullOrEmpty(RunData.Identifier))
                RunData.Identifier = PseudonymEntry.Text;

            if (String.IsNullOrEmpty(RunData.Credential))
                RunData.Credential = PasswordEntry.Text;

            bool isValid = EntityRunFormData.Validate(RunData, out validationMessages);
            if (isValid)
            {
                ValidationMessage = "";
                LbValidation.Text = ValidationMessage;
                WwwFeedback feedback = await restService.SubmitRun(RunData);
                if (feedback == null)
                {
                    // ValidationMessage = "Fehler beim übertragen der Daten. Bitte versuche es später noch einmal.";
                    // LbValidation.Text = ValidationMessage;
                    OfflineLayout.IsVisible = true;
                    SubmitLayout.IsVisible = false;
                }
                else
                {
                    if (!String.IsNullOrEmpty(feedback.Error))
                    {
                        ValidationMessage = feedback.Error;
                        LbValidation.Text = ValidationMessage;
                    }
                    else
                    {
                        SubmitLayout.IsVisible = false;
                        SetFeedbackGrid(feedback);
                        Walk.CurrentDistance = Math.Round(feedback.CurrentDistance, 2);
                        SetCurrentText();
                    }
                }
                MapLayout.IsVisible = true;
            }
            else
            {
                ValidationMessage = String.Join(Environment.NewLine, validationMessages);
                LbValidation.Text = ValidationMessage;
            }
            InitMap();
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }

        private async void TryGoOnline(object sender, EventArgs e)
        {
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;
            Walk = await restService.GetWalk(walkGuid);
            SetCurrentText();
            if (Run.StopTime != DateTime.MinValue)
            {
                if (await InitRunData())
                {
                    OfflineLayout.IsVisible = false;
                }
            }
            else if (!String.IsNullOrEmpty(Walk.Name))
                OfflineLayout.IsVisible = false;

            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }

        private void LocationManager_LocationUpdated(object sender, Models.LocationUpdatedEventArgs e)
        {
            if (Run.RunItems.Count == 0 && !String.IsNullOrEmpty(e.Error))
            {
                StopRun(null, null);
                LbTime.Text = e.Error;
                return;
            }
            else
            {
                Run.RunItems.Add(new RunItem()
                {
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    Accuracy = e.Accuracy == null ? 0 : (double)e.Accuracy,
                    TimeStamp = e.Timestamp,
                    Error = e.Error
                });

                if (Run.RunItems.Count % 10 == 0)
                {
                    var distance = Run.GetStepInfo();
                    if ((DateTime.UtcNow - Run.StartTime) > App.MaxRunTime)
                    {
                        StopRun(null, null);
                        LbTime.Text = "Der Lauf wurde nach 2h gestoppt.";
                        return;
                    }

                    //if (Run.AverageSpeedInKmH > App.MaxSpeedInKmH)
                    //{
                    //    StopRun(null, null);
                    //    LbTime.Text = "Die maximal erlaubte Geschwindigkeit wurde überschritten.";
                    //    return;
                    //}
                    LbTime.Text = $"Laufzeit: {(DateTime.UtcNow - Run.StartTime).ToString(@"hh\:mm\:ss")}";
                    LbDistance.Text = "Zurückgelegt: " + distance + " m";
                }
            }
        }

        private void PseudonymEntry_Completed(object sender, EventArgs e)
        {
            RunData.Identifier = ((Entry)sender).Text;
            bool isValid = EntityRunFormData.Validate(RunData, out validationMessages);
            if (isValid)
                ValidationMessage = "";
            else
                ValidationMessage = String.Join(Environment.NewLine, validationMessages);

            LbValidation.Text = ValidationMessage;
        }

        private void PasswordEntry_Completed(object sender, EventArgs e)
        {
            RunData.Credential = ((Entry)sender).Text;
            bool isValid = EntityRunFormData.Validate(RunData, out validationMessages);
            if (isValid)
                ValidationMessage = "";
            else
                ValidationMessage = String.Join(Environment.NewLine, validationMessages);

            LbValidation.Text = ValidationMessage;
        }

        private void PseudonymEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunData.Identifier = PseudonymEntry.Text;
        }

        private void PasswordEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunData.Credential = PasswordEntry.Text;
        }

        void InitMap()
        {
            grMap.Children.Clear();
            MapLayout.IsVisible = true;
            MapWebView wvMap = new MapWebView()
            {
                Source = Run.Html,
                HeightRequest = 500,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            grMap.Children.Add(wvMap);
            grMap.ResolveLayoutChanges();
        }

        async Task<bool> InitRunData()
        {
            if (String.IsNullOrEmpty(Walk.Name))
            {
                Walk = await restService.GetWalk(walkGuid);
                if (String.IsNullOrEmpty(Walk.Name))
                {
                    OfflineLayout.IsVisible = true;
                    return false;
                }
                OfflineLayout.IsVisible = false;
                SetCurrentText();
            }
            RunData = new EntityRunFormData()
            {
                Walk = walkGuid,
                School = Walk.Schools.First().Name,
                // Distance = (float)(Run.Distance / 1000),
                Distance = 2.2f,
                Time = Run.StopTime
            };
            InitClassPicker();
            SubmitLayout.IsVisible = true;
            scrollView.ResolveLayoutChanges();
            return true;
        }

        void InitClassPicker()
        {
            var selected = RunData.SchoolClass;
            ClassPicker.Title = "Klasse";
            ClassPicker.Items.Clear();
            foreach (var ent in Walk.Schools.First(f => f.Name == RunData.School).SchoolClasses.Select(s => s.Name).OrderBy(o => o))
            {
                ClassPicker.Items.Add(ent);
            }
            ClassPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (ClassPicker.SelectedIndex == -1)
                {
                    RunData.SchoolClass = "";
                }
                else
                {
                    RunData.SchoolClass = ClassPicker.Items[ClassPicker.SelectedIndex];
                }
            };
            if (!String.IsNullOrEmpty(selected))
            {
                if (ClassPicker.Items.Contains(selected))
                {
                    ClassPicker.SelectedIndex = ClassPicker.Items.IndexOf(selected);
                }
            };
        }

        void SetFeedbackGrid(WwwFeedback feedback)
        {
            FeedbackGrid.Children.Clear();
            FeedbackGrid.RowDefinitions.Clear();
            FeedbackGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < 5; i++)
                FeedbackGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < 4; i++)
                FeedbackGrid.ColumnDefinitions.Add(new ColumnDefinition());

            FeedbackGrid.Children.Add(new Label
            {
                Text = ""
            }, 0, 0);
            FeedbackGrid.Children.Add(new Label
            {
                Text = "km"
            }, 1, 0);
            FeedbackGrid.Children.Add(new Label
            {
                Text = "%"
            }, 2, 0);
            FeedbackGrid.Children.Add(new Label
            {
                Text = "Position"
            }, 3, 0);

            FeedbackGrid.Children.Add(new Label
            {
                Text = "Deine Strecke"
            }, 0, 1);
            FeedbackGrid.Children.Add(new Label
            {
                Text = feedback.EntTotal.ToString()
            }, 1, 1);
            FeedbackGrid.Children.Add(new Label
            {
                Text = $"{feedback.EntPercentage} %"
            }, 2, 1);
            FeedbackGrid.Children.Add(new Label
            {
                Text = $"{feedback.EntPosition}"
            }, 3, 1);

            FeedbackGrid.Children.Add(new Label
            {
                Text = "Deine Klasse"
            }, 0, 2);
            FeedbackGrid.Children.Add(new Label
            {
                Text = feedback.ClassTotal.ToString()
            }, 1, 2);
            FeedbackGrid.Children.Add(new Label
            {
                Text = $"{feedback.ClassPercentage} %"
            }, 2, 2);
            FeedbackGrid.Children.Add(new Label
            {
                Text = $"{feedback.ClassPosition}"
            }, 3, 2);

            FeedbackGrid.Children.Add(new Label
            {
                Text = "Dein Jahrgang"
            }, 0, 3);
            FeedbackGrid.Children.Add(new Label
            {
                Text = feedback.YearTotal.ToString()
            }, 1, 3);
            FeedbackGrid.Children.Add(new Label
            {
                Text = $"{feedback.YearPercentage} %"
            }, 2, 3);
            FeedbackGrid.Children.Add(new Label
            {
                Text = $"{feedback.YearPosition}"
            }, 3, 3);

            FeedbackLayout.IsVisible = true;
        }

        void SetCurrentText()
        {
            LbWalk.Text = Walk.Name;
            LbCurrent.Text = $"Es wurden {Walk.CurrentDistance} von {Walk.TotalDistance} km zurückgelegt.";
        }

        public void Dispose()
        {
            if (locationManager != null)
            {
                locationManager.StopLocationUpdates();
                locationManager.LocationUpdated -= LocationManager_LocationUpdated;
            }
            PasswordEntry.Completed -= PasswordEntry_Completed;
            PseudonymEntry.Completed -= PseudonymEntry_Completed;
            PasswordEntry.TextChanged -= PasswordEntry_TextChanged;
            PseudonymEntry.TextChanged -= PseudonymEntry_TextChanged;
        }
    }
}
