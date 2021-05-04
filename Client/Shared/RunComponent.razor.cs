
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using www.pwa.Client.Models;
using www.pwa.Client.Services;
using www.pwa.Shared;

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
        }

        private void Error(string err)
        {
            debugInfo = err;
            InvokeAsync(() => StateHasChanged());
        }

        public void Dispose() {
            runService.Stop();
            runService.RunStopped -= onRunStopped;
            runService.RunDataAvailable -= onRunDataAvailable;
        }
    }
}