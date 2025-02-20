using IServiceProvider = MvvmToolkit.Core.Ioc;

namespace MvvmToolkit.Core.Ioc.Hosting
{
    public interface IHost : IDisposable
    {
        IServiceProvider Services { get; }
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
