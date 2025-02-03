using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MvvmToolkit.App.Helper;
using MvvmToolkit.App.Logs;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

namespace MvvmToolkit.App
{
    public abstract class GenericHostApplication : Application
    {
        private static IHost? Host { get; set; }
        private ILogger<GenericHostApplication>? _logger;
        private bool _canGenerateDump;
        private CoreDumpHelper.MiniDumpType _dumpType = CoreDumpHelper.MiniDumpType.MiniDumpNormal;
        /// <summary>
        /// Dump가 발생했을때 덤프를 저장할 위치를 얻어옵니다.
        /// </summary>
        /// <returns>Dump의 저장 위치</returns>
        protected virtual string GetDumpPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            string? dirPath = Path.GetDirectoryName(assembly?.Location);
            string exeName = AppDomain.CurrentDomain.FriendlyName;
            string dateTime = DateTime.Now.ToString("[yyyy-MM-dd][HH-mm-ss-fff]", CultureInfo.InvariantCulture);

            return $"{dirPath}/[{exeName}]{dateTime}.dmp";
        }
        /// <summary>
        /// Crash 발생 시 Dump를 생성할 수 있는 옵션을 설정합니다.
        /// </summary>
        /// <param name="canGenerateDump">덤프를 생성할지 여부.</param>
        /// <param name="dumpType">생성할 덤프의 타입</param>
        protected void SetDumpOption(bool canGenerateDump, CoreDumpHelper.MiniDumpType dumpType = CoreDumpHelper.MiniDumpType.MiniDumpNormal)
        {
            _canGenerateDump = canGenerateDump;
            _dumpType = dumpType;
        }
        protected GenericHostApplication()
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
            Host = builder
              //.UseDefaultServiceProvider(ConfigureServiceProvider)
              .ConfigureAppConfiguration(ConfigureAppConfiguration)
              //.ConfigureLogging(ConfigureLogging)
              .ConfigureServices(ConfigureServices)
              .Build();

            ContainerProvider.Initialize(Host.Services);

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Dispatcher.UnhandledExceptionFilter += Dispatcher_UnhandledExceptionFilter;

            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // "LogOptions" 섹션을 IOptions 패턴으로 등록
            services.Configure<LogOptions>(context.Configuration.GetSection("LogOptions"));

            // CustomLoggerProvider를 싱글톤으로 등록
            services.AddSingleton<ILoggerProvider, CustomLoggerProvider>();

            // 로깅 시스템에 CustomLoggerProvider 추가
            services.AddLogging(logging =>
            {
                logging.ClearProviders(); // 기본 로거 제거 (선택 사항)
                logging.AddProvider(new CustomLoggerProvider(context.Configuration.GetSection("LogOptions").Get<LogOptions>() ?? new LogOptions()));
            });            
        }

        private void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true)
                .Build();

            // 구성 정보를 빌드하여 IConfiguration 객체를 생성합니다.
            IConfiguration configuration = builder.Build();

            // "LogOptions" 섹션을 LogOptions 클래스에 바인딩합니다.
            var logOptions = configuration.GetSection("LogOptions").Get<LogOptions>() ?? new LogOptions();

        }

        /// <summary>
        /// Dispatchers the unhandled exception filter using the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Dispatcher_UnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {
            try
            {
                e.RequestCatch = true;

                _logger?.LogError(e.Exception, $"[App Error Catch] {e.Exception}");
                _logger?.LogError(e.Exception, $"[App_DispatcherUnhandledExceptionFilter] {e.Exception.Message}");
            }
            catch
            {
                // ignored
            }
        }
        /// <summary>
        /// Dispatchers the unhandled exception using the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;

                _logger?.LogError(e.Exception, $"[App Error Catch] {e.Exception}");
                _logger?.LogError(e.Exception, $"[App_DispatcherUnhandledException] {e.Exception.Message}");
                if (_canGenerateDump)
                {
                    string dumpPath = GetDumpPath();
                    CoreDumpHelper.CreateMemoryDump(_dumpType, dumpPath);
                }
            }
            catch
            {
                // ignored
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true)
                .Build();
            // LogOptions 섹션을 읽어 LogOptions 객체에 바인딩합니다.
            var logOptions = configuration.GetSection("LogOptions").Get<LogOptions>() ?? new LogOptions();

            // DI 컨테이너 설정
            var serviceProvider = Host.Services.
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton(logOptions)
                // CustomLoggerProvider 등록 (필요에 따라 싱글톤이나 다른 수명 주기로 등록)
                .AddSingleton<ILoggerProvider>(sp => new CustomLoggerProvider(sp.GetRequiredService<LogOptions>()))
                // 로깅 시스템에 CustomLoggerProvider 추가
                .AddLogging(builder => builder.AddProvider(new CustomLoggerProvider(logOptions)))
                .BuildServiceProvider();

            Services = serviceProvider;

            base.OnStartup(e);
        }
    }
}
