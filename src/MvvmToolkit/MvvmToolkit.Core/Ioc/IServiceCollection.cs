﻿
namespace MvvmToolkit.Core.Ioc
{
    public interface IServiceCollection2
    {
        public void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface;
        public void AddSingleton<TImplementation>() where TImplementation : class;
        public void AddSingleton<TImplementation>(TImplementation implementation) where TImplementation : class;
        public void AddSingleton<TInterface, TImplementation>(Func<IServiceContainer2, TInterface> factory) where TImplementation : TInterface;
        public void AddSingleton<TImplementation>(Func<IServiceContainer2, TImplementation> factory) where TImplementation : class;

        public void AddTransient<TInterface, TImplementation>() where TImplementation : TInterface;
        public void AddTransient<TImplementation>() where TImplementation : class;
        public void AddTransient<TImplementation>(TImplementation implementation) where TImplementation : class;
        public void AddTransient<TInterface, TImplementation>(Func<IServiceContainer2, TInterface> factory) where TImplementation : TInterface;
        public void AddTransient<TImplementation>(Func<IServiceContainer2, TImplementation> factory) where TImplementation : class;

        public bool CheckType(Type type);
        public Type KeyType(string name);
        public ServiceType GetType(Type type);
        public IServiceContainer2 CreateContainer();
    }
}
