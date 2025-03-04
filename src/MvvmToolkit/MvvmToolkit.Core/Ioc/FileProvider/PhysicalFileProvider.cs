using MvvmToolkit.Core.Ioc.FileProvider.Physical;
using MvvmToolkit.Core.Ioc.FileProvider.Physical.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.FileProvider
{
    public class PhysicalFileProvider : IFileProvider, IDisposable
    {
        private readonly ExclusionFilters _filters;
        private readonly Func<PhysicalFilesWatcher> _fileWatcherFactory;
        private PhysicalFilesWatcher? _fileWatcher;
        private bool? _usePollingFileWatcher;
        private bool? _useActivePolling;
        private bool _disposed;
        /// <summary>
        /// The root directory for this instance.
        /// </summary>
        public string Root { get; }
        public bool UsePollingFileWatcher
        {
            get
            {
                if (_fileWatcher != null)
                {
                    return false;
                }
                if (_usePollingFileWatcher == null)
                {
                    //ReadPollingEnvironmentVariables();
                }
                return _usePollingFileWatcher ?? false;
            }
            set
            {
                if (_fileWatcher != null)
                {
                    throw new Exception("");
                }
                _usePollingFileWatcher = value;
            }
        }
        public bool UseActivePolling
        {
            get
            {
                if (_useActivePolling == null)
                {
                    // ReadPollingEnvironmentVariables();
                }

                return _useActivePolling.Value;
            }

            set => _useActivePolling = value;
        }
        public PhysicalFileProvider(string root)
            : this(root , ExclusionFilters.Sensitive)
        {
            
        }

        public PhysicalFileProvider(string root, ExclusionFilters sensitive)
        {
            if(Path.IsPathRooted(root) == false)
            {
                throw new ArgumentException("The path must be rooted", nameof(root));
            }
            string fullRoot = Path.GetFullPath(root);

            Root = PathUtils.EnsureTrailingSlash(fullRoot);
            if (!Directory.Exists(Root))
            {
                throw new DirectoryNotFoundException(Root);
            }
            this._filters = sensitive;
            _fileWatcherFactory = CreateFileWatcher;
        }
        internal PhysicalFilesWatcher CreateFileWatcher()
        {
            string root = PathUtils.EnsureTrailingSlash(Path.GetFullPath(Root));
            FileSystemWatcher? watcher;
            watcher = UsePollingFileWatcher && UseActivePolling ? null : new FileSystemWatcher(root);
            return new PhysicalFilesWatcher(root, watcher, UsePollingFileWatcher, _filters)
            {
                UseActivePolling = UseActivePolling,
            };
        }
    }
}
