using Microsoft.Extensions.Logging;
using PeriodicTimer = MvvmToolkit.Core.Threadings.PeriodicTimer;

namespace MvvmToolkit.Core.Tasks.Worker
{
    public interface IPeriodicTimerWorker : IDisposable
    {
        TimeSpan Cycle { get; set; }
        void StartWorker(TimeSpan timespan, Func<Task> worker);
        void StopWorker();
    }
}
