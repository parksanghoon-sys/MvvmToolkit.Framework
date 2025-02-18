using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace MvvmToolkit.Core.Logs
{

    public class CustomLoggerFactory : ILoggerFactory
    {
        private readonly IOptions<LoggerOptions> _options;
        private readonly ConcurrentDictionary<string, CustomLogger> _loggerCache = new ();

        public CustomLoggerFactory(IOptions<LoggerOptions> options)
        {
            _options = options;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return _loggerCache.GetOrAdd(categoryName, name => new CustomLogger(name, _options));
        }

        public void AddProvider(ILoggerProvider provider) { }

        public void Dispose() { }
    }

}
