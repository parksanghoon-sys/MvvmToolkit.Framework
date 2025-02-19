

using Microsoft.Extensions.Configuration;

namespace MvvmToolkit.Core.Ioc.Hosting
{
    public static class Host
    {
        public static IHostBuilder CreateDefaultBuilder() =>
                    CreateDefaultBuilder(args: null);
        public static IHostBuilder CreateDefaultBuilder(string[]? args)
        {
            HostBuilder builder = new();
            return builder.ConfigureDefaults(args);
        }
    }
    public class HostBuilder : IHostBuilder
    {
        public IDictionary<object, object> Properties => throw new NotImplementedException();

        public IHost Build()
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            throw new NotImplementedException();
        }
    }
}
