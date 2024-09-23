using Microsoft.Extensions.DependencyInjection;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Autofac
{
    public static class IOC_DependencyInjectionManager
    {
        /// <summary>
        /// 注入的服务容器
        /// </summary>
        public static IServiceProvider Current { get; set; } = default!;

        public static T GetService<T>() => Current.GetRequiredService<T>();

        public static bool IsRegistered<T>() => Current.GetService(typeof(T)) != null;

        public static bool IsRegistered(Type type) => Current.GetService(type) != null;

        public static object GetService(Type type) => Current.GetService(type);

        public static T GetService<T>(params object[] parameters)
        {
            var service = Current.GetService(typeof(T));
            if (service != null)
            {
                return (T)service;
            }

            throw new Exception($"Service of type {typeof(T)} is not registered.");
        }
    }
}