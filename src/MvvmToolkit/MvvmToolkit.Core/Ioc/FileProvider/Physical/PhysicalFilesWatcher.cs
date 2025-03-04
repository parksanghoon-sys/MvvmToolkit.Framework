using MvvmToolkit.Core.Ioc.Internal;
using MvvmToolkit.Core.Ioc.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvvmToolkit.Core.Ioc.FileProvider.Physical
{
    public class PhysicalFilesWatcher : IDisposable
    {
        private static readonly Action<object?> _cancelTokenSource = state => ((CancellationTokenSource?)state)!.Cancel();
        internal static TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(4);

        private readonly ConcurrentDictionary<string, ChangeTokenInfo> _filePathTokenLookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, ChangeTokenInfo> _wildcardTokenLookup = new(StringComparer.OrdinalIgnoreCase);

        private readonly FileSystemWatcher? _fileWatcher;
        private readonly object _fileWatcherLock = new();
        private readonly string _root;
        private readonly ExclusionFilters _filters;

        private Timer? _timer;
        private bool _timerInitialized;
        private object _timerLock = new();
        private readonly Func<Timer> _timerFactory;
        private bool _disposed;

        public PhysicalFilesWatcher(string root, FileSystemWatcher? fileSystemWatcher, bool polForChanges)
            :this(root, fileSystemWatcher, polForChanges, ExclusionFilters.Sensitive)
        {
            
        }

        public PhysicalFilesWatcher(string root, FileSystemWatcher? fileSystemWatcher, bool polForChanges, ExclusionFilters filters)
        {
            _root = root;
            if(fileSystemWatcher is not null)
            {
                _fileWatcher = fileSystemWatcher;
                _fileWatcher.IncludeSubdirectories = true;
                //_fileWatcher.Created += OnChanged;
                //_fileWatcher.Changed += OnChanged;
                //_fileWatcher.Renamed += OnRenamed;
                //_fileWatcher.Deleted += OnChanged;
                //_fileWatcher.Error += OnError;
            }
            PollForChanges = polForChanges;
            _filters = filters;

            PollingChangeTokens = new ConcurrentDictionary<IPollingChangeToken, IPollingChangeToken>();
            _timerFactory = () => NonCapturingTimer.Create(RaiseChangeEvents, state: PollingChangeTokens, dueTime: TimeSpan.Zero, period: DefaultPollingInterval);
        }
        internal bool PollForChanges { get; }

        internal bool UseActivePolling { get; set; }
        internal static void RaiseChangeEvents(object? state)
        {
            Debug.Assert(state != null);

            // Iterating over a concurrent bag gives us a point in time snapshot making it safe
            // to remove items from it.
            var changeTokens = (ConcurrentDictionary<IPollingChangeToken, IPollingChangeToken>)state;
            foreach (System.Collections.Generic.KeyValuePair<IPollingChangeToken, IPollingChangeToken> item in changeTokens)
            {
                IPollingChangeToken token = item.Key;

                if (!token.HasChanged)
                {
                    continue;
                }

                if (!changeTokens.TryRemove(token, out _))
                {
                    // Move on if we couldn't remove the item.
                    continue;
                }

                // We're already on a background thread, don't need to spawn a background Task to cancel the CTS
                try
                {
                    token.CancellationTokenSource!.Cancel();
                }
                catch
                {

                }
            }
        }
        internal ConcurrentDictionary<IPollingChangeToken, IPollingChangeToken> PollingChangeTokens { get; }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        /// <param name="disposing"><c>true</c> is invoked from <see cref="IDisposable.Dispose"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _fileWatcher?.Dispose();
                    _timer?.Dispose();
                }
                _disposed = true;
            }
        }
        private readonly struct ChangeTokenInfo
        {
            public ChangeTokenInfo(
                CancellationTokenSource tokenSource,
                CancellationChangeToken changeToken)
                : this(tokenSource, changeToken, matcher: null)
            {
            }

            public ChangeTokenInfo(
                CancellationTokenSource tokenSource,
                CancellationChangeToken changeToken,
                Matcher? matcher)
            {
                TokenSource = tokenSource;
                ChangeToken = changeToken;
                Matcher = matcher;
            }

            public CancellationTokenSource TokenSource { get; }

            public CancellationChangeToken ChangeToken { get; }

            public Matcher? Matcher { get; }
          
        }
    }
    public class Matcher
    {

    }

}
