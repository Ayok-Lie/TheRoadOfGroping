using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Model.Interface;
using RoadOfGroping.Model.Modules;

namespace RoadOfGroping.Model.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加一个对象访问器，如果该对象已经注册过，则抛出异常。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>对象访问器</returns>
        public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services)
        {
            // 检查是否已经注册过该对象
            if (services.Any(s => s.ServiceType == typeof(IObjectAccessor<T>)))
            {
                throw new Exception("该对象已经注册成功过 " + typeof(T).AssemblyQualifiedName);
            }
            // 创建一个新的对象访问器
            var accessor = new ObjectAccessor<T>();
            // 将对象访问器添加到服务集合的开头，以便快速检索
            services.Insert(0, ServiceDescriptor.Singleton<IObjectAccessor<T>>(accessor));

            // 再次检查是否已经注册过该对象
            if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
            {
                throw new Exception("该对象已经注册成功过: " + typeof(T).AssemblyQualifiedName);
            }

            // 将对象访问器添加到服务集合的开头
            services.Insert(0, ServiceDescriptor.Singleton(accessor));

            return accessor;
        }

        /// <summary>
        /// 添加一个带有指定对象的对象访问器，如果该对象已经注册过，则抛出异常。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="obj">要注册的对象</param>
        /// <returns>对象访问器</returns>
        public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, T obj)
        {
            // 检查是否已经注册过该对象
            if (services.Any(s => s.ServiceType == typeof(IObjectAccessor<T>)))
            {
                throw new Exception("该对象已经注册成功过 " + typeof(T).AssemblyQualifiedName);
            }
            // 创建一个新的对象访问器，并设置其对象
            var accessor = new ObjectAccessor<T>(obj);
            // 将对象访问器添加到服务集合的开头，以便快速检索
            services.Insert(0, ServiceDescriptor.Singleton<IObjectAccessor<T>>(accessor));

            // 再次检查是否已经注册过该对象
            if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
            {
                throw new Exception("该对象已经注册成功过: " + typeof(T).AssemblyQualifiedName);
            }

            // 将对象访问器添加到服务集合的开头
            services.Insert(0, ServiceDescriptor.Singleton(accessor));

            return accessor;
        }

        /// <summary>
        /// 检查服务集合是否为 null，如果为 null 则抛出异常。
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void CheckNull(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentException("services is null");
            }
        }

        /// <summary>
        /// 获取单例实例，如果找不到则返回 null。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>单例实例或 null</returns>
        public static T? GetSingletonInstanceOrNull<T>(this IServiceCollection services) where T : class
        {
            // 查找服务类型为 T 的服务描述符
            var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T));
            // 返回服务描述符的实现实例，并进行类型转换
            return serviceDescriptor?.ImplementationInstance as T;
        }

        /// <summary>
        /// 检查服务集合是否为 null，如果为 null 则抛出异常。
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void ChcekNull(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException("IServiceCollection为空");
            }
        }

        /// <summary>
        /// 获取单例实例，如果找不到则抛出异常。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>单例实例</returns>
        public static T GetSingletonInstance<T>(this IServiceCollection services) where T : class
        {
            // 获取单例实例或 null
            var service = services.GetSingletonInstanceOrNull<T>();
            // 如果服务为 null，则抛出异常
            if (service == null)
            {
                throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
            }

            return service;
        }

        /// <summary>
        /// 添加指定类型的程序集中的服务。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddAssemblyOf<T>(this IServiceCollection services)
        {
            // 添加指定类型的程序集
            return services.AddAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        /// <summary>
        /// 添加指定程序集中的服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="assembly">程序集</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly)
        {
            // 使用常规注册器添加程序集中的服务
            new ConventionalRegistrar().AddAssembly(services, assembly);

            return services;
        }
    }
}
