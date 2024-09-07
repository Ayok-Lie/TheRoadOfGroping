using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoadOfGroping.Common.Reflection;
using RoadOfGroping.Utility.DependencyInjection;

namespace RoadOfGroping.Common.DependencyInjection
{
    /// <summary>
    /// 常规注册器，用于将程序集中的类型注册到依赖注入容器中。
    /// </summary>
    public class ConventionalRegistrar
    {
        /// <summary>
        /// 将指定程序集中的所有类型注册到服务集合中。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="assembly">要注册的程序集</param>
        public void AddAssembly(IServiceCollection services, Assembly assembly)
        {
            // 获取程序集中的所有类型，并过滤出非抽象、非泛型的类
            var types = AssemblyHelper
                .GetAllTypes(assembly)
                .Where(
                    type => type != null &&
                            type.IsClass &&
                            !type.IsAbstract &&
                            !type.IsGenericType
                ).ToArray();

            // 将过滤后的类型注册到服务集合中
            AddTypes(services, types);
        }

        /// <summary>
        /// 将指定类型注册到服务集合中。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="type">要注册的类型</param>
        public void AddType(IServiceCollection services, Type type)
        {
            // 获取类型的 RegisterLifeAttribute 属性
            var registerLifeAttribute = GetRegisterLifeAttributeOrNull(type);

            // 获取类型的生命周期
            var lifeTime = GetLifeTimeOrNull(type, registerLifeAttribute);

            // 如果生命周期为空，则不进行注册
            if (lifeTime == null)
            {
                return;
            }

            // 获取类型的默认服务类型
            var exposedServiceTypes = GetDefaultServices(type);

            // 遍历服务类型，将其注册到服务集合中
            foreach (var exposedServiceType in exposedServiceTypes)
            {
                var serviceDescriptor = ServiceDescriptor.Describe(
                    exposedServiceType,//服务类型
                    type,//实现类型
                    lifeTime.Value//生命周期
                );

                // 根据 RegisterLifeAttribute 的属性决定注册方式
                if (registerLifeAttribute?.ReplaceServices == true)
                {
                    services.Replace(serviceDescriptor);
                }
                else if (registerLifeAttribute?.TryRegister == true)
                {
                    services.TryAdd(serviceDescriptor);
                }
                else
                {
                    services.Add(serviceDescriptor);
                }
            }
        }

        /// <summary>
        /// 将多个类型注册到服务集合中。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="types">要注册的类型数组</param>
        public void AddTypes(IServiceCollection services, params Type[] types)
        {
            foreach (var type in types)
            {
                AddType(services, type);
            }
        }

        /// <summary>
        /// 获取指定类型的 RegisterLifeAttribute 属性。
        /// </summary>
        /// <param name="type">要获取属性的类型</param>
        /// <returns>RegisterLifeAttribute 属性，如果没有则返回 null</returns>
        protected virtual RegisterLifeAttribute GetRegisterLifeAttributeOrNull(Type type)
        {
            return type.GetCustomAttribute<RegisterLifeAttribute>(true);
        }

        /// <summary>
        /// 获取指定类型的生命周期。
        /// </summary>
        /// <param name="type">要获取生命周期的类型</param>
        /// <param name="registerLifeAttribute">RegisterLifeAttribute 属性</param>
        /// <returns>生命周期，如果没有则返回 null</returns>
        protected ServiceLifetime? GetLifeTimeOrNull(Type type, RegisterLifeAttribute registerLifeAttribute)
        {
            return registerLifeAttribute?.Lifetime ?? GetServiceLifetime(type);
        }

        /// <summary>
        /// 获取指定类型的默认生命周期。
        /// </summary>
        /// <param name="type">要获取生命周期的类型</param>
        /// <returns>生命周期，如果没有则返回 null</returns>
        protected ServiceLifetime? GetServiceLifetime(Type type)
        {
            if (typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Transient;
            }

            if (typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Singleton;
            }

            if (typeof(IScopedDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Scoped;
            }

            return null;
        }

        /// <summary>
        /// 获取指定类型的默认服务类型。
        /// </summary>
        /// <param name="type">要获取服务类型的类型</param>
        /// <returns>服务类型列表</returns>
        private static List<Type> GetDefaultServices(Type type)
        {
            var serviceTypes = new List<Type>();

            // 添加类型本身
            serviceTypes.AddIfNotContains(type);

            // 遍历类型的接口，添加符合命名规则的接口
            foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
            {
                var interfaceName = interfaceType.Name;

                if (interfaceName.StartsWith("I"))
                {
                    interfaceName = Right(interfaceName, interfaceName.Length - 1);
                }

                if (type.Name.EndsWith(interfaceName))
                {
                    serviceTypes.Add(interfaceType);
                }
            }

            return serviceTypes;
        }

        /// <summary>
        /// 获取字符串的右侧部分。
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="len">要获取的长度</param>
        /// <returns>字符串的右侧部分</returns>
        /// <exception cref="ArgumentException">如果 len 的长度大于字符串长度，则抛出异常</exception>
        public static string Right(string str, int len)
        {
            if (str.Length < len)
            {
                throw new ArgumentException("len的长度不能大于字符串长度！");
            }

            return str.Substring(str.Length - len, len);
        }
    }
}
