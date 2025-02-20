using MvvmToolkit.Core.Ioc.Configurations;
using MvvmToolkit.Core.Ioc.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MvvmToolkit.Core.Ioc.Hosting.Extentions
{
    public static class HostingHostBuilderExtensions
    {
        /// <summary>
        /// Configures an existing <see cref="IHostBuilder"/> instance with pre-configured defaults. This will overwrite
        /// previously configured values and is intended to be called before additional configuration calls.
        /// </summary>
        /// <remarks>
        ///   The following defaults are applied to the <see cref="IHostBuilder"/>:
        ///     * set the <see cref="IHostEnvironment.ContentRootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/>
        ///     * load host <see cref="IConfiguration"/> from "DOTNET_" prefixed environment variables
        ///     * load host <see cref="IConfiguration"/> from supplied command line args
        ///     * load app <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostEnvironment.EnvironmentName"/>].json'
        ///     * load app <see cref="IConfiguration"/> from User Secrets when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development' using the entry assembly
        ///     * load app <see cref="IConfiguration"/> from environment variables
        ///     * load app <see cref="IConfiguration"/> from supplied command line args
        ///     * configure the <see cref="ILoggerFactory"/> to log to the console, debug, and event source output
        ///     * enables scope validation on the dependency injection container when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development'
        /// </remarks>
        /// <param name="builder">The existing builder to configure.</param>
        /// <param name="args">The command line args.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder ConfigureDefaults(this IHostBuilder builder, string[]? args)
        {
            return builder.ConfigureAppConfiguration((hostingContext, config) => ApplyDefaultAppConfiguration(hostingContext, config, args))
                          .ConfigureServices(AddDefaultServices));
        }

        internal static void ApplyDefaultAppConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder appConfigBuilder, string[]? args)
        {
            IHostEnvironment env = hostingContext.HostingEnvironment;
            bool reloadOnChange = GetReloadConfigOnChangeValue(hostingContext);

            appConfigBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);

            if (env.IsDevelopment() && env.ApplicationName is { Length: > 0 })
            {
                try
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    appConfigBuilder.AddUserSecrets(appAssembly, optional: true, reloadOnChange: reloadOnChange);
                }
                catch (FileNotFoundException)
                {
                    // The assembly cannot be found, so just skip it.
                }
            }

            appConfigBuilder.AddEnvironmentVariables();

            AddCommandLineConfig(appConfigBuilder, args);
#if NETSTANDARD

#else
            [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Calling IConfiguration.GetValue is safe when the T is bool.")]
#endif
            static bool GetReloadConfigOnChangeValue(HostBuilderContext hostingContext) => hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
        }
    }
}
