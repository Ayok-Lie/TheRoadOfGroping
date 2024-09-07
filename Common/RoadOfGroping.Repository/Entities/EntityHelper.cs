namespace RoadOfGroping.Repository.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using global::RoadOfGroping.Repository.Auditing;

    namespace RoadOfGroping.Repository.Auditing
    {
        /// <summary>
        /// 实体帮助类，提供与实体相关的实用方法。
        /// </summary>
        public static class EntityHelper
        {
            /// <summary>
            /// 检查给定的类型是否实现了 IEntity 接口。
            /// </summary>
            /// <param name="type">要检查的类型</param>
            /// <returns>如果类型实现了 IEntity 接口，则返回 true，否则返回 false</returns>
            public static bool IsEntity(Type type)
            {
                return typeof(IEntity).IsAssignableFrom(type);
            }

            /// <summary>
            /// 检查给定的对象是否是值对象。
            /// </summary>
            /// <param name="obj">要检查的对象</param>
            /// <returns>如果对象是值对象，则返回 true，否则返回 false</returns>
            public static bool IsValueObject(object obj)
            {
                return obj != null && IsValueObject(obj.GetType());
            }

            /// <summary>
            /// 检查给定的类型是否是实体类型。如果不是，则抛出异常。
            /// </summary>
            /// <param name="type">要检查的类型</param>
            /// <exception cref="Exception">如果类型不是实体类型，则抛出异常</exception>
            public static void CheckEntity(Type type)
            {
                if (!IsEntity(type))
                {
                    throw new Exception($"Given {nameof(type)} is not an entity: {type.AssemblyQualifiedName}. It must implement {typeof(IEntity).AssemblyQualifiedName}.");
                }
            }

            /// <summary>
            /// 检查给定的类型是否实现了 IEntity&lt;TKey&gt; 接口。
            /// </summary>
            /// <param name="type">要检查的类型</param>
            /// <returns>如果类型实现了 IEntity&lt;TKey&gt; 接口，则返回 true，否则返回 false</returns>
            public static bool IsEntityWithId(Type type)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.GetTypeInfo().IsGenericType &&
                        interfaceType.GetGenericTypeDefinition() == typeof(IEntity<>))
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 检查实体的 ID 是否为默认值。
            /// </summary>
            /// <typeparam name="TKey">实体 ID 的类型</typeparam>
            /// <param name="entity">要检查的实体</param>
            /// <returns>如果实体的 ID 为默认值，则返回 true，否则返回 false</returns>
            public static bool HasDefaultId<TKey>(IEntity<TKey> entity)
            {
                if (EqualityComparer<TKey>.Default.Equals(entity.Id, default))
                {
                    return true;
                }

                // 针对 EF Core 的特殊处理，当附加到 DbContext 时，EF Core 会将 int/long 类型的 ID 设置为最小值
                if (typeof(TKey) == typeof(int))
                {
                    return Convert.ToInt32(entity.Id) <= 0;
                }

                if (typeof(TKey) == typeof(long))
                {
                    return Convert.ToInt64(entity.Id) <= 0;
                }

                return false;
            }

            /// <summary>
            /// 尝试为实体设置 ID。
            /// </summary>
            /// <typeparam name="TKey">实体 ID 的类型</typeparam>
            /// <param name="entity">要设置 ID 的实体</param>
            /// <param name="idFactory">用于生成 ID 的工厂方法</param>
            /// <param name="checkForDisableIdGenerationAttribute">是否检查禁用 ID 生成的属性</param>
            public static void TrySetId<TKey>(
                IEntity<TKey> entity,
                Func<TKey> idFactory,
                bool checkForDisableIdGenerationAttribute = false)
            {
                ObjectPropertyHelper.TrySetProperty(
                    entity,
                    x => x.Id,
                    idFactory, new Type[] { });
            }
        }
    }
}