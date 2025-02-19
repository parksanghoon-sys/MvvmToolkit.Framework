
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using IServiceProvider = MvvmToolkit.Core.Ioc;

namespace MvvmToolkit.Core.Ioc.Hosting
{
    public interface IHost : IDisposable
    {
        IServiceProvider Services { get; }
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
    public interface IHostBuilder
    {
        IDictionary<object, object> Properties { get; }
        IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate);
        IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);
        IHost Build();
    }
    /// <summary>
    /// Context containing the common services on the <see cref="IHost" />. Some properties may be null until set by the <see cref="IHost" />.
    /// </summary>
    public class HostBuilderContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HostBuilderContext"/>.
        /// </summary>
        /// <param name="properties">A non-null <see cref="IDictionary{TKey, TValue}"/> for sharing state between components during the host building process.</param>
        public HostBuilderContext(IDictionary<object, object> properties)
        {            
            Properties = properties;
        }

        /// <summary>
        /// The <see cref="IHostEnvironment" /> initialized by the <see cref="IHost" />.
        /// </summary>
        public IHostEnvironment HostingEnvironment { get; set; } = null!;

        /// <summary>
        /// The <see cref="IConfiguration" /> containing the merged configuration of the application and the <see cref="IHost" />.
        /// </summary>
        public IConfiguration Configuration { get; set; } = null!;

        /// <summary>
        /// A central location for sharing state between components during the host building process.
        /// </summary>
        public IDictionary<object, object> Properties { get; }
    }
}
