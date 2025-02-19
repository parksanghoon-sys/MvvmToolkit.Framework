﻿namespace MvvmToolkit.Core.Ioc
{
    public interface IServiceProvider
    {   
        object? GetService(Type serviceType);
    }
    public class ContainerProvider : IServiceProvider
    {
        /// <summary>
        /// `Provider`는 서비스 제공자를 저장하는 필드입니다.<br/>
        /// </summary>
        private static IServiceCollection? Provider;

        public static void Initialize(IServiceCollection? provider)
        {
            Provider = provider;
        }
        /// <summary>
        /// `Resolve` 메서드는 주어진 타입의 서비스를 해결하고 반환합니다.<br/>
        /// <param name="type">해결할 서비스의 타입입니다.</param>
        /// <param name="serviceKey">해결할 서비스를 찾을 수 있는 키입니다.</param>
        /// <returns>해결된 서비스 객체를 반환합니다. 서비스를 해결할 수 없는 경우 null을 반환합니다.</returns>
        /// </summary>
        public static object? Resolve(Type? type, object? serviceKey = null)
        {
            if (Provider is not null)
                if (Provider.CheckType(type: type) == true)
                    return Provider.CreateContainer().GetService(type);
            return null;
        }
        /// <summary>
        /// `Resolve{T}` 메서드는 주어진 타입의 서비스를 해결하고 반환합니다.<br/>
        /// <typeparam name="T">해결할 서비스의 타입입니다.</typeparam>
        /// <param name="serviceKey">해결할 서비스를 찾을 수 있는 키입니다.</param>
        /// <returns>해결된 서비스 객체를 반환합니다. 서비스를 해결할 수 없는 경우 null을 반환합니다.</returns>
        /// </summary>
        public static T? Resolve<T>(object? serviceKey = null)
        {
            if (Provider == null)
            {
                return default;
            }

            return (T?)Resolve(typeof(T), serviceKey);
        }

        public object? GetService(Type serviceType)
        {
            if (Provider is not null)
                if (Provider.CheckType(type: serviceType) == true)
                    return Provider.CreateContainer().GetService(serviceType);
            return null;
        }
    }
}
