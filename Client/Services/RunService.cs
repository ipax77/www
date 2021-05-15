
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using www.pwa.Client.Models;
using System;
using System.Threading;

namespace www.pwa.Client.Services
{
    public delegate void Notify();
    public class RunService
    {
        private readonly IJSRuntime js;
        private readonly ILogger<RunService> logger;
        public Run Run { get; set; } = new Run();
        private System.Timers.Timer Timer;
        AutoResetEvent autoEvent = new AutoResetEvent(false);
        private int timerCount;
        public event Notify RunStopped;
        public event Notify RunDataAvailable;
        public RunStatus runStatus { get; set; } = RunStatus.Init;
        private int Interval;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private CancellationToken Token;

        public RunService(IJSRuntime js, ILogger<RunService> logger)
        {
            this.js = js;
            this.logger = logger;
        }

        public async Task<string> Start(CancellationToken token, int interval = 1000)
        {
            Interval = interval;
            Token = token;
            string getLocation = await js.InvokeAsync<string>("GetLocation");

            Run = new Run();
            Run.RunInfo.StartTime = DateTime.UtcNow;
            // Timer = new Timer(onTimerElapsed, autoEvent, 1000, Interval);
            Timer = new System.Timers.Timer(Interval);
            Timer.Elapsed += onTimerElapsed;
            Timer.AutoReset = true;
            Timer.Enabled = true;
            runStatus = RunStatus.Running;
            OnRunDataAvailable();
            return getLocation;
        }

        public async Task Stop()
        {
            if (Timer != null)
            {
                Timer.Enabled = false;
                Timer.Dispose();
                timerCount = 0;
                await js.InvokeVoidAsync("GetLocation");
                await Task.Delay(3000);
            }
            Run.RunInfo.StopTime = DateTime.UtcNow;
            Run.GetFinalRunInfo();
            runStatus = RunStatus.FinishedAndTransmitting;
            OnRunStopped();
        }

        public async Task<string> StartWatch(CancellationToken token)
        {
            Token = token;
            Run Run = new Run();
            string getLocation = await js.InvokeAsync<string>("StartRun");
            Run.RunInfo.StartTime = DateTime.UtcNow;
            OnRunDataAvailable();
            return getLocation;
        }

        public async Task StopWatch()
        {
            await js.InvokeVoidAsync("StopRun");
            Run.RunInfo.StopTime = DateTime.UtcNow;
            Run.GetFinalRunInfo();
            runStatus = RunStatus.FinishedAndTransmitting;
            OnRunStopped();
        }

        private async void onTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Interlocked.Increment(ref timerCount);
            if (timerCount > 7200000 / Interval)
            {
                logger.LogInformation($"Run stopped due to MaxTime {timerCount * Interval} > {7200000 / Interval}");
                Stop();
            }
            else
            {
                await semaphoreSlim.WaitAsync();
                if (Token.IsCancellationRequested) {
                    return;
                }
                await js.InvokeVoidAsync("GetLocation");
                if (timerCount % (60000 / Interval) == 0)
                {
                    Run.GetRunInfo();
                    if (Run.RunInfo.Distance > 42.195 * 1000.0) {
                        Run.RunInfo._Distance = 42.1 * 1000.0 / 6376500.0;
                        logger.LogInformation($"Run stopped due to MaxDistance {Run.RunInfo.Distance} > {42.195 * 1000.0}");
                        Stop();
                    } else
                        OnRunDataAvailable();
                } else {
                    // DEBUG
                    // OnRunDataAvailable();
                }
            }
        }

        public void AddRunItem(double[] jsdata)
        {
            Run.RunItems.Add(new RunItem(jsdata[0], jsdata[1], jsdata[2], jsdata[3], jsdata[4]));
            //semaphoreSlim.Release();
        }

        public void Error() {
            //semaphoreSlim.Release();
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