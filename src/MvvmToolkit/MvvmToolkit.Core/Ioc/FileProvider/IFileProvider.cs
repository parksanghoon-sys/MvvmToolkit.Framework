using MvvmToolkit.Core.Ioc.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.FileProvider
{
    /// <summary>
    /// A read-only file provider abstraction.
    /// </summary>
    public interface IFileProvider
    {
        IFileInfo GetFileInfo(string subpath);
        IDirectoryContents GetDirectoryContents(string subpath);
        IChangeToken Watch(string filter);
    }
}
