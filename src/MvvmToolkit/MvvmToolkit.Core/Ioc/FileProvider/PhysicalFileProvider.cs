using MvvmToolkit.Core.Ioc.FileProvider.Physical;
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
        /// <summary>
        /// The root directory for this instance.
        /// </summary>
        public string Root { get; }
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

            Root = fullRoot;
            this._filters = sensitive;
            _fileWatcherFactory = CreateFileWatcher;
        }
    }
}
