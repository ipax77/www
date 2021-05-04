
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using www.pwa.Client.Models;
using System.Timers;
using System;

namespace www.pwa.Client.Services
{
    public delegate void Notify();
    public class RunService
    {
        private readonly IJSRuntime js;
        private readonly ILogger<RunService> logger;
        public Run Run { get; set; } = new Run();
        private Timer Timer;
        private int timerCount;
        public event Notify RunStopped;
        public event Notify RunDataAvailable;
        public RunStatus runStatus { get; set; } = RunStatus.Init;

        public RunService(IJSRuntime js, ILogger<RunService> logger)
        {
            this.js = js;
            this.logger = logger;
        }

        public void Start()
        {
            Run = new Run();
            Run.RunInfo.StartTime = DateTime.UtcNow;
            Timer = new Timer();
            Timer.Interval = 3000;
            Timer.Elapsed += onTimerElapsed;
            Timer.AutoReset = true;
            Timer.Enabled = true;
            runStatus = RunStatus.Running;
            OnRunDataAvailable();
        }

        public void Stop()
        {
            if (Timer != null)
            {
                Timer.Enabled = false;
                Timer.Elapsed -= onTimerElapsed;
                timerCount = 0;
                Timer.Dispose();
                Timer = null;
            }
            Run.GetFinalRunInfo();
            runStatus = RunStatus.FinishedAndTransmitting;
            OnRunStopped();
        }

        private async void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timerCount++;
            if (timerCount > 2400)
            {
                Stop();
            }
            else
            {
                await js.InvokeVoidAsync("GetLocation");
                if (timerCount % 20 == 0)
                {
                    Run.GetRunInfo();
                    if (Run.RunInfo.Distance > 42.195 * 1000) {
                        Run.RunInfo._Distance = 42.1 * 1000.0 / 6376500.0;
                        Stop();
                    } else
                        OnRunDataAvailable();
                } else {
                    // DEBUG
                    OnRunDataAvailable();
                }
            }
        }

        public void AddRunItem(double[] jsdata)
        {
            Run.RunItems.Add(new RunItem(jsdata[0], jsdata[1], jsdata[2], jsdata[3], jsdata[4]));
        }

        protected virtual void OnRunStopped()
        {
            RunStopped?.Invoke();
        }

        protected virtual void OnRunDataAvailable()
        {
            RunDataAvailable?.Invoke();
        }

    }
}