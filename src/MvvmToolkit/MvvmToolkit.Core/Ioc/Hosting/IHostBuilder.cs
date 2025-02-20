using MvvmToolkit.Core.Ioc.Configurations;

namespace MvvmToolkit.Core.Ioc.Hosting
{
    public interface IHostBuilder
    {
        IDictionary<object, object> Properties { get; }
        IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate);
        IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate);
        IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);
        IHost Build();
    }
}
