namespace MvvmToolkit.Core.Ioc.Configurations
{
    public abstract class StreamConfigurationProvider : ConfigurationProvider
    {
        private bool _loaded;
        public StreamConfigurationSource Source { get; }
        public StreamConfigurationProvider(StreamConfigurationSource source)        
        {
            Source = source;
        }
        public abstract void Load(Stream stream);
        //
        // 요약:
        //     Load the configuration data from the stream. Will throw after the first call.
        public override void Load()
        {        
            Load(this.Source.Stream);
            _loaded = true;
        }
    }
}
