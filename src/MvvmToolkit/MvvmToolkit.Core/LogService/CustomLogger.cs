using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;

namespace MvvmToolkit.Core.Logs
{
    public partial class CustomLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly LoggerOptions _options;
        private readonly object _lock = new();

        public CustomLogger(string categoryName, IOptions<LoggerOptions> options)
        {
            _categoryName = categoryName;
            _options = options.Value;
        }

        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName} - {formatter(state, exception)}";

            switch (_options.OutputType)
            {
                case LogOutputType.Console:
                    Console.WriteLine(logMessage);
                    break;

                case LogOutputType.File:
                    lock (_lock)
                    {
                        File.AppendAllText(_options.FilePath, logMessage + Environment.NewLine);
                    }
                    break;

                case LogOutputType.Tcp:
                    await SendLogToTcpAsync(logMessage);
                    break;
            }
        }

        private async Task SendLogToTcpAsync(string message)
        {
            try
            {
                if (_options.TcpHost == null)
                    return;

                using TcpClient client = new(_options.TcpHost, _options.TcpPort);
                byte[] data = Encoding.UTF8.GetBytes(message);
                await client.GetStream().WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Logger Error] Failed to send log via TCP: {ex.Message}");
            }
        }
    }
}
