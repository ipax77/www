using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        private WalkAppModel Walk;
        private EntityRunFormData RunData;
        private string validationMessage = String.Empty;
        private ICollection<string> validationMessages = new List<string>();

        public string ValidationMessage
        {
            get { return validationMessage; }
            set
            {
                validationMessage = value;
                OnPropertyChanged(nameof(ValidationMessage));
            }
        }

        public MainPage()
        {
            InitializeComponent();
            restService = new RestService();
            GetWalk();
            Debug();
        }

        private async void Debug()
        {
            if (Walk == null)
            {
                LbTime.Text = "Fehler beim Laden.";
                return;
            }
            Run = await restService.GetDebugData();
            string finfo = Run.SetRunInfo();
            if (!String.IsNullOrEmpty(finfo))
            {
                LbTime.Text = finfo;
                return;
            }
            MapWebView wvMap = new MapWebView()
            {
                Source = Run.Html,
                HeightRequest = 500,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            grMap.Children.Add(wvMap);
            MapLayout.IsVisible = true;
        }

        private async void GetWalk()
        {
            Walk = await restService.GetWalk();
            SetCurrentText();
        }

        private void StartRun(object sender, EventArgs e)
        {
            SubmitLayout.IsVisible = false;
            MapLayout.IsVisible = false;
            FeedbackGrid.IsVisible = false;
            grMap.Children.Clear();
            LbTime.Text = "";
            LbDistance.Text = "";
            LbCount.Text = "";
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
            BtnStart.IsEnabled = true;
            BtnStart.Clicked += StartRun;
            BtnStop.IsEnabled = false;
            BtnStop.Clicked -= StopRun;
            locationManager.StopLocationUpdates();
            locationManager.LocationUpdated -= LocationManager_LocationUpdated;
            Run.StopTime = DateTime.UtcNow;
            restService = new RestService();

            if (sender != null)
            {
                LbTime.Text = $"Laufzeit: {(Run.StopTime - Run.StartTime).ToString(@"hh\:mm\:ss")}";
            }
            LbDistance.Text = "Zurückgelegt: " + Math.Round(Run.Distance, 2).ToString() + " m";

            InitRunData();

            // DEBUG
            await restService.SubmitDebugData(Run);
            LbCount.Text = Run.RunItems.Count.ToString();
        }

        async void Submit_Clicked(object sender, EventArgs e)
        {
            bool isValid = EntityRunFormData.Validate(RunData, out validationMessages);
            if (isValid)
            {
                ValidationMessage = "";
                WwwFeedback feedback = await restService.SubmitRun(RunData);
                if (feedback == null)
                {
                    ValidationMessage = "Fehler beim übertragen der Daten. Bitte versuche es später noch einmal.";
                }
                else
                {
                    if (!String.IsNullOrEmpty(feedback.Error))
                    {
                        ValidationMessage = feedback.Error;
                    }
                    else
                    {
                        SubmitLayout.IsVisible = false;
                        SetFeedbackGrid(feedback);
                        Walk.CurrentDistance = feedback.CurrentDistance;
                        SetCurrentText();
                    }
                }
                MapLayout.IsVisible = true;
            }
            else
                ValidationMessage = String.Join(Environment.NewLine, validationMessages);
        }

        private void LocationManager_LocationUpdated(object sender, Models.LocationUpdatedEventArgs e)
        {
            if (Run.RunItems.Count == 0 && !String.IsNullOrEmpty(e.Error))
            {
                StopRun(null, null);
                LbTime.Text = e.Error;
                return;
            } else
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
                    Run.SetRunInfo();
                    if ((DateTime.UtcNow - Run.StartTime) > App.MaxRunTime)
                    {
                        StopRun(null, null);
                        LbTime.Text = "Der Lauf wurde nach 2h gestoppt.";
                        return;
                    }

                    if (Run.AverageSpeedInKmH > App.MaxSpeedInKmH)
                    {
                        StopRun(null, null);
                        LbTime.Text = "Die maximal erlaubte Geschwindigkeit wurde überschritten.";
                        return;
                    }
                    LbTime.Text = $"Laufzeit: {(DateTime.UtcNow - Run.StartTime).ToString(@"hh\:mm\:ss")}";
                    LbDistance.Text = "Zurückgelegt: " + Math.Round(Run.Distance, 2).ToString() + " m";
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

        }

        private void PasswordEntry_Completed(object sender, EventArgs e)
        {
            RunData.Credential = ((Entry)sender).Text;
        }

        void InitRunData()
        {
            RunData = new EntityRunFormData()
            {
                Walk = Walk.Name,
                School = Walk.Schools.First().Name,
            };
            InitClassPicker();
        }

        void InitClassPicker()
        {
            ClassPicker.Title = "Klasse";
            ClassPicker.Items.Clear();
            foreach (var ent in Walk.Schools.First(f => f.Name == RunData.School).SchoolClasses.Select(s => s.Name))
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

            FeedbackGrid.IsVisible = true;
        }

        void SetCurrentText()
        {
            LbCurrent.Text = $"Es wurden {Walk.CurrentDistance} von {Walk.TotalDistance} km zurückgelet.";
        }

        public void Dispose()
        {
            if (locationManager != null)
            {
                locationManager.StopLocationUpdates();
                locationManager.LocationUpdated -= LocationManager_LocationUpdated;
            }
        }
    }
}
