using MvvmToolkit.Core.Ioc.FileProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Configurations
{
    public static class FileConfigurationExtensions
    {
        private const string FileProviderKey = "FileProvider";
        private const string FileLoadExceptionHandlerKey = "FileLoadExceptionHandler";


        /// <summary>
        /// Gets the default <see cref="IFileProvider"/> to be used for file-based providers.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>The default <see cref="IFileProvider"/>.</returns>
        public static IFileProvider GetFileProvider(this IConfigurationBuilder builder)
        {            

            if (builder.Properties.TryGetValue(FileProviderKey, out object? provider))
            {
                return (IFileProvider)provider;
            }

            return new PhysicalFileProvider(AppContext.BaseDirectory ?? string.Empty);
        }
    }
}
