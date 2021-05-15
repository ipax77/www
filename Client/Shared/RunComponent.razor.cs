
using System.Threading;
using System.Net;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using www.pwa.Client.Models;
using www.pwa.Client.Services;
using www.pwa.Shared;
using System.Collections.Generic;

namespace www.pwa.Client.Shared
{
    public partial class RunComponent : ComponentBase, IDisposable
    {
        [Inject]
        protected RunService runService { get; set; }
        [Inject]
        protected DataService dataService { get; set; }
        private static Action<double[]> action;
        private static Action<string> actionErr;
        private string debugInfo;
        private UploadStatus UploadStatus = UploadStatus.UploadDone;
        private List<string> errors = new List<string>();
        private CancellationTokenSource Source;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            action = runService.AddRunItem;
            actionErr = Error;
            runService.RunStopped += onRunStopped;
            runService.RunDataAvailable += onRunDataAvailable;
        }

        private void onRunDataAvailable()
        {
            InvokeAsync(() => StateHasChanged());
        }

        private void onRunStopped()
        {
            if (runService.Run.RunInfo.Distance < 100) {
                Error("Zu wenige Daten :(");
                UploadStatus = UploadStatus.UploadFailed;
                runService.runStatus = RunStatus.Failed;
                InvokeAsync(() => StateHasChanged());
            } else 
                SubmitData();
        }

        [JSInvokable]
        public static void UpdateRunCaller(double[] pos)
        {
            action.Invoke(pos);
        }

        [JSInvokable]
        public static void ErrorCaller(string err)
        {
            actionErr.Invoke(err);
        }

        private async Task StartRun() {
            Source = new CancellationTokenSource();
            string runStart = await runService.Start(Source.Token);
            if (!String.IsNullOrEmpty(runStart)) {
                Error(runStart);
                await runService.Stop();
                runService.runStatus = RunStatus.Failed;
            }
            await Task.Run( async() => {
                while (true) {
                    await Task.Delay(1000, Source.Token);
                    if (Source.Token.IsCancellationRequested)
                        break;
                    await InvokeAsync(() => StateHasChanged());
                }
            });
        }

        private async Task StopRun() {
            Source.Cancel();
            UploadStatus = UploadStatus.Uploading;
            runService.runStatus = RunStatus.Paused;
            await InvokeAsync(() => StateHasChanged());
            await runService.Stop();
        }

        public async void StartWatch() {
            Source = new CancellationTokenSource();
            string runStart = await runService.StartWatch(Source.Token);
            if (!String.IsNullOrEmpty(runStart)) {
                Error(runStart);
                runService.runStatus = RunStatus.Failed;
            }
        }

        public async void StopWatch() {
            Source.Cancel();
            UploadStatus = UploadStatus.Uploading;
            runService.runStatus = RunStatus.Paused;
            await InvokeAsync(() => StateHasChanged());
            await runService.StopWatch();
        }

        private async Task SubmitData()
        {
            UploadStatus = UploadStatus.Uploading;
            await InvokeAsync(() => StateHasChanged());
            EntityRunFormData data = new EntityRunFormData()
            {
                School = "TestSchool",
                Walk = "Weltumrundung",
                Identifier = "Anonymouse",
                SchoolClass = "Lehrer",
                Distance = (float)runService.Run.RunInfo.Distance,
                Time = runService.Run.RunInfo.StartTime,
                Credential = "test123"
            };
            runService.Run.RunInfo.Feedback = await dataService.Submit(data);
            if (runService.Run.RunInfo.Feedback == null)
                UploadStatus = UploadStatus.UploadFailed;
            else
                UploadStatus = UploadStatus.UploadSuccess;
            await InvokeAsync(() => StateHasChanged());

            // DEBUG
            RunDebugModel debug = new RunDebugModel() {
                Start = runService.Run.RunInfo.StartTime,
                Stop = runService.Run.RunInfo.StopTime,
                Errors = errors,
                RunDebugItems = runService.Run.RunItems.Select(s => new RunDebugItemModel() {
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Accuracy = s.Accuracy,
                    TimeStamp = s.TimeStamp,
                    Speed = s.Speed
                }).ToList()
            };
            await dataService.SendDebugRun(debug);
        }

        private void Error(string err)
        {
            debugInfo = err;
            errors.Add(debugInfo);
            InvokeAsync(() => StateHasChanged());
        }

        public void Dispose() {
            Source?.Cancel();
            runService.Stop();
            runService.RunStopped -= onRunStopped;
            runService.RunDataAvailable -= onRunDataAvailable;
        }
    }
}