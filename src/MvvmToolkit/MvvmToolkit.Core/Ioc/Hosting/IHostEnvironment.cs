using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Hosting
{
    public interface IHostEnvironment
    {
        string EnvironmentName { get; set; }
        string ApplicationName { get; set; }
        string ContentRootPath { get; set; }        
    }
}
