﻿using MvvmToolkit.Core.Ioc.FileProvider;
using MvvmToolkit.Core.Ioc.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Configurations
{
    public abstract class FileConfigurationSource : IConfigurationSource
    {
        public IFileProvider FileProvider { get; set; }
#if NETSTANDARD        
#elif NETCOREAPP
        [DisallowNull]
#endif
        public string? Path { get; set; }
        public bool Optional { get; set; }
        public bool ReloadOnChange { get; set; }
        public int ReloadDelay { get; set; } = 250;
        public Action<FileLoadExceptionContext>? OnLoadException { get; set; }
        public abstract IConfigurationProvider Build(IConfigurationBuilder builder);
        public void EnsureDefault(IConfigurationBuilder builder)
        {
            FileProvider ??= builder.GetFileProvider();
            // OnLoadException ??= builder.GetFileLoadExceptionHandler();
        }
        public void ResolveFileProvider()
        {
            if (FileProvider is null && string.IsNullOrEmpty(Path) == false && System.IO.Path.IsPathRooted(Path))
            {
                string? directory = System.IO.Path.GetDirectoryName(Path);
                string? pathToFile = System.IO.Path.GetFileName(Path);

                while(string.IsNullOrEmpty(directory) == false && Directory.Exists(directory) == false)
                {
                    pathToFile = System.IO.Path.Combine(System.IO.Path.GetFileName(directory), pathToFile);
                    directory = System.IO.Path.GetDirectoryName(directory);
                }
                if (Directory.Exists(directory))
                {
                    FileProvider = new PhysicalFileProvider(directory);
                    Path = pathToFile;
                }
            }
            
        }
    }
    /// <summary>
    /// Base class for file based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public abstract class FileConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly IDisposable? _changeTokenRegistration;

        /// <summary>
        /// Initializes a new instance with the specified source.
        /// </summary>
        /// <param name="source">The source settings.</param>
        public FileConfigurationProvider(FileConfigurationSource source)
        {
            Source = source;

            if (Source.ReloadOnChange && Source.FileProvider != null)
            {
                _changeTokenRegistration = ChangeToken.OnChange(
                    () => Source.FileProvider.Watch(Source.Path!),
                    () =>
                    {
                        Thread.Sleep(Source.ReloadDelay);
                        Load(reload: true);
                    });
            }
        }

        /// <summary>
        /// The source settings for this provider.
        /// </summary>
        public FileConfigurationSource Source { get; }

        /// <summary>
        /// Generates a string representing this provider name and relevant details.
        /// </summary>
        /// <returns> The configuration name. </returns>
        public override string ToString()
            => $"{GetType().Name} for '{Source.Path}' ({(Source.Optional ? "Optional" : "Required")})";

        private void Load(bool reload)
        {
            IFileInfo? file = Source.FileProvider?.GetFileInfo(Source.Path ?? string.Empty);
            if (file == null || !file.Exists)
            {
                if (Source.Optional || reload) // Always optional on reload
                {
                    Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    var error = new StringBuilder(SR.Format(SR.Error_FileNotFound, Source.Path));
                    if (!string.IsNullOrEmpty(file?.PhysicalPath))
                    {
                        error.Append(SR.Format(SR.Error_ExpectedPhysicalPath, file.PhysicalPath));
                    }
                    HandleException(ExceptionDispatchInfo.Capture(new FileNotFoundException(error.ToString())));
                }
            }
            else
            {
                static Stream OpenRead(IFileInfo fileInfo)
                {
                    if (fileInfo.PhysicalPath != null)
                    {
                        // The default physical file info assumes asynchronous IO which results in unnecessary overhead
                        // especially since the configuration system is synchronous. This uses the same settings
                        // and disables async IO.
                        return new FileStream(
                            fileInfo.PhysicalPath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite,
                            bufferSize: 1,
                            FileOptions.SequentialScan);
                    }

                    return fileInfo.CreateReadStream();
                }

                using Stream stream = OpenRead(file);
                try
                {
                    Load(stream);
                }
                catch (Exception ex)
                {
                    if (reload)
                    {
                        Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                    }
                    var exception = new InvalidDataException(SR.Format(SR.Error_FailedToLoad, file.PhysicalPath), ex);
                    HandleException(ExceptionDispatchInfo.Capture(exception));
                }
            }
            // REVIEW: Should we raise this in the base as well / instead?
            OnReload();
        }

        /// <summary>
        /// Loads the contents of the file at <see cref="Path"/>.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">Optional is <c>false</c> on the source and a
        /// directory cannot be found at the specified Path.</exception>
        /// <exception cref="FileNotFoundException">Optional is <c>false</c> on the source and a
        /// file does not exist at specified Path.</exception>
        /// <exception cref="InvalidDataException">An exception was thrown by the concrete implementation of the
        /// <see cref="Load()"/> method. Use the source <see cref="FileConfigurationSource.OnLoadException"/> callback
        /// if you need more control over the exception.</exception>
        public override void Load()
        {
            Load(reload: false);
        }

        /// <summary>
        /// Loads this provider's data from a stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        public abstract void Load(Stream stream);

        private void HandleException(ExceptionDispatchInfo info)
        {
            bool ignoreException = false;
            if (Source.OnLoadException != null)
            {
                var exceptionContext = new FileLoadExceptionContext
                {
                    Provider = this,
                    Exception = info.SourceException
                };
                Source.OnLoadException.Invoke(exceptionContext);
                ignoreException = exceptionContext.Ignore;
            }
            if (!ignoreException)
            {
                info.Throw();
            }
        }

        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Dispose the provider.
        /// </summary>
        /// <param name="disposing"><c>true</c> if invoked from <see cref="IDisposable.Dispose"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            _changeTokenRegistration?.Dispose();
        }
    }
}
