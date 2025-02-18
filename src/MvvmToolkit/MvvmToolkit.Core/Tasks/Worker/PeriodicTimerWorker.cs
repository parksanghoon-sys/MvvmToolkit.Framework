using Microsoft.Extensions.Logging;
using PeriodicTimer = MvvmToolkit.Core.Threadings.PeriodicTimer;

namespace MvvmToolkit.Core.Tasks.Worker
{
    public class PeriodicTimerWorker : IPeriodicTimerWorker
    {
        private string? _timerName;
        private PeriodicTimer? _periodicTimer;
        private CancellationTokenSource? _cts;
        private Func<Task> _work;

        private Task? periodicTask;
        private ILogger<PeriodicTimerWorker> _logger;
        private TimeSpan _cycle;
        public TimeSpan Cycle
        {
            get => _cycle;
            set
            {
                if(_cycle != value && _cts != null)
                {
                    StopWorker();
                    StartWorker(value, _work);
                }
                _cycle = value;
            }
        }
        public PeriodicTimerWorker(string timerName, ILogger<PeriodicTimerWorker> logger)
        {
            _timerName = timerName;
            _logger = logger;
        }
        public void StartWorker(TimeSpan timespan, Func<Task> worker)
        {
            _periodicTimer = new PeriodicTimer(timespan);
            _work = worker;

            if (_cts == null || _cts?.IsCancellationRequested == true)
            {
                _cts = new CancellationTokenSource();
                periodicTask = Task.Factory.StartNew(_ => ActionPeriodicTask(_cts.Token), TaskCreationOptions.LongRunning);
                _logger.LogInformation($"Start {_timerName} Timer");
            }
        }        
        public void StopWorker()
        {
            Task.Run(StopAndReleaseTask);
        }
        private async Task StopAndReleaseTask()
        {
            if (_cts?.IsCancellationRequested == false)
            {
                _cts.Cancel();
                if (periodicTask != null)
                    await periodicTask;
                _cts?.Dispose();
                _periodicTimer?.Dispose();
                _cts = null;
                _periodicTimer = null;
            }
        }
        public void Dispose()
        {
            StopWorker();
        }
        private async Task ActionPeriodicTask(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_periodicTimer != null && await _periodicTimer.WaitForNextTickAsync(token).ConfigureAwait(false))
                    {
                        if (_work != null) await _work().ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException oce)
            {
                _logger?.LogInformation(oce, "{TimerName} {Message}", _timerName, oce.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "{TimerName} {Message}", this._timerName, ex.ToString());
            }

            _logger?.LogInformation("{TimerName} Finished", _timerName);
        }
    }
}
