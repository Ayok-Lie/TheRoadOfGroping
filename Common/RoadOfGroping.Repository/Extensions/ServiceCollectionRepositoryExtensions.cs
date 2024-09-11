using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoadOfGroping.Repository.Entities;
using RoadOfGroping.Repository.Repository;

namespace RoadOfGroping.Repository.Extensions
{
    public static class ServiceCollectionRepositoryExtensions
    {
        /// <summary>
        /// 尝试添加仓储服务。
        /// </summary>
        /// <typeparam name="TDbContext">数据库上下文类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <param name="assemblies">程序集集合。</param>
        /// <param name="types">实体类型集合。</param>
        /// <returns>服务集合。</returns>
        public static IServiceCollection TryAddRepository<TDbContext>(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies,
            IEnumerable<Type>? types)
            where TDbContext : DbContext
        {
            // 获取所有导出的类型
            var allTypes = assemblies.Where(assembly => !assembly.IsDynamic)
                                     .SelectMany(assembly => assembly.GetExportedTypes())
                                     .ToList();

            // 获取实体类型
            var entityTypes = types ?? allTypes.Where(type => type.IsEntity());

            foreach (var entityType in entityTypes)
            {
                // 创建仓储接口类型
                var repositoryInterfaceType = typeof(IBaseRepository<>).MakeGenericType(entityType);

                // 添加默认仓储实现
                services.TryAddAddDefaultRepository(repositoryInterfaceType, GetRepositoryImplementationType(typeof(TDbContext), entityType));

                // 添加自定义仓储实现
                services.TryAddCustomRepository(repositoryInterfaceType, allTypes);

                // 如果实体类型实现了 IEntity<TKey> 接口
                if (typeof(IEntity<>).IsGenericInterfaceAssignableFrom(entityType))
                {
                    // 获取主键类型
                    var fieldType = entityType.GetProperty("Id")!.PropertyType;

                    // 创建带有主键类型的仓储接口类型
                    repositoryInterfaceType = typeof(IBaseRepository<,>).MakeGenericType(entityType, fieldType);

                    // 添加默认仓储实现
                    services.TryAddAddDefaultRepository(repositoryInterfaceType, GetRepositoryImplementationType(typeof(TDbContext), entityType, fieldType));

                    // 添加自定义仓储实现
                    services.TryAddCustomRepository(repositoryInterfaceType, allTypes);
                }
            }

            return services;
        }

        /// <summary>
        /// 判断类型是否为实体类型。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <returns>是否为实体类型。</returns>
        private static bool IsEntity(this Type type)
            => type.IsClass && !type.IsGenericType && !type.IsAbstract && typeof(IEntity).IsAssignableFrom(type);

        /// <summary>
        /// 尝试添加自定义仓储实现。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="repositoryInterfaceType">仓储接口类型。</param>
        /// <param name="allTypes">所有类型。</param>
        private static void TryAddCustomRepository(this IServiceCollection services, Type repositoryInterfaceType, List<Type> allTypes)
        {
            // 获取自定义仓储接口类型
            var customRepositoryInterfaceTypes = allTypes.Where(type
                => type.GetInterfaces().Any(t => t == repositoryInterfaceType) && type.IsInterface && !type.IsGenericType);

            foreach (var customRepositoryInterfaceType in customRepositoryInterfaceTypes)
            {
                // 获取自定义仓储实现类型
                var customRepositoryImplementationTypes =
                    allTypes.Where(type => type.IsClass && customRepositoryInterfaceType.IsAssignableFrom(type)).ToList();

                // 确保自定义仓储实现类型只有一个
                if (customRepositoryImplementationTypes.Count != 1)
                {
                    throw new NotSupportedException(
                        $"The number of types of {customRepositoryInterfaceType.FullName} implementation classes must be 1");
                }

                // 添加自定义仓储实现
                services.TryAddScoped(customRepositoryInterfaceType, customRepositoryImplementationTypes.FirstOrDefault()!);
            }
        }

        /// <summary>
        /// 尝试添加默认仓储实现。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="repositoryInterfaceType">仓储接口类型。</param>
        /// <param name="repositoryImplementationType">仓储实现类型。</param>
        private static void TryAddAddDefaultRepository(this IServiceCollection services, Type repositoryInterfaceType,
            Type repositoryImplementationType)
        {
            // 确保仓储接口类型可以分配给仓储实现类型
            if (repositoryInterfaceType.IsAssignableFrom(repositoryImplementationType))
            {
                services.TryAddScoped(repositoryInterfaceType, repositoryImplementationType);
            }
        }

        /// <summary>
        /// 获取仓储实现类型。
        /// </summary>
        /// <param name="dbContextType">数据库上下文类型。</param>
        /// <param name="entityType">实体类型。</param>
        /// <returns>仓储实现类型。</returns>
        private static Type GetRepositoryImplementationType(Type dbContextType, Type entityType)
            => typeof(RoadOfGropingRepository<,>).MakeGenericType(dbContextType, entityType);

        /// <summary>
        /// 获取带有主键类型的仓储实现类型。
        /// </summary>
        /// <param name="dbContextType">数据库上下文类型。</param>
        /// <param name="entityType">实体类型。</param>
        /// <param name="keyType">主键类型。</param>
        /// <returns>仓储实现类型。</returns>
        private static Type GetRepositoryImplementationType(Type dbContextType, Type entityType, Type keyType)
            => typeof(RoadOfGropingRepository<,,>).MakeGenericType(dbContextType, entityType, keyType);

        /// <summary>
        /// 判断类型是否为具体类型（非抽象类和接口）。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <returns>是否为具体类型。</returns>
        public static bool IsConcrete(this Type type) => !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;

        /// <summary>
        /// 判断类型是否实现了泛型接口。
        /// </summary>
        /// <param name="eventHandlerType">泛型接口类型。</param>
        /// <param name="type">类型。</param>
        /// <returns>是否实现了泛型接口。</returns>
        public static bool IsGenericInterfaceAssignableFrom(this Type eventHandlerType, Type type) =>
            type.IsConcrete() &&
            type.GetInterfaces().Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == eventHandlerType);
    }
}
