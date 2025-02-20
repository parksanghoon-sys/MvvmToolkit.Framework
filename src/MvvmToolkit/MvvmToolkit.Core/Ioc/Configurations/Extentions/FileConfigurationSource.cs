using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Configurations.Extentions
{
    public abstract class FileConfigurationSource : IConfigurationSource
    {

        public abstract IConfigurationProvider Build(IConfigurationBuilder builder);
      
    }
}
