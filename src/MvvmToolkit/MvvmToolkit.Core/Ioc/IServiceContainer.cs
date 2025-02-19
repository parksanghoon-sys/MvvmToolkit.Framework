namespace MvvmToolkit.Core.Ioc
{
    public interface IServiceContainer
    {
        public TInterface GetService<TInterface>() where TInterface : class;
        public object GetService(Type serviceType);
        public Type TypeGet(string key);
    }
}
