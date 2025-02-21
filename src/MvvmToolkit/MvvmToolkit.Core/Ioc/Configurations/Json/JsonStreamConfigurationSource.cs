using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Configurations.Json
{
    public class JsonStreamConfigurationSource : StreamConfigurationSource
    {
        /// <summary>
        /// Builds the <see cref="JsonStreamConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>An <see cref="JsonStreamConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
            => new JsonStreamConfigurationProvider(this);
    }
    public class JsonStreamConfigurationProvider : StreamConfigurationProvider
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="source">The <see cref="JsonStreamConfigurationSource"/>.</param>
        public JsonStreamConfigurationProvider(JsonStreamConfigurationSource source)
            : base(source)
        {
        }
        public override void Load(Stream stream)
        {
            
        }
    }
}
