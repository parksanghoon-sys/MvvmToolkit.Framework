using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Configurations
{
    public abstract class StreamConfigurationSource : IConfigurationSource
    {
        public Stream? Stream
        {
            get;
            set;
        }

        public abstract IConfigurationProvider Build(IConfigurationBuilder builder);        
    }
}
