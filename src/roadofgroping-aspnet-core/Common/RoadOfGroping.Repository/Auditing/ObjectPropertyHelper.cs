using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using RoadOfGroping.Common.Extensions;

namespace RoadOfGroping.Repository.Auditing
{
    /// <summary>
    /// 对象属性帮助类，用于设置对象的属性值。
    /// </summary>
    public static class ObjectPropertyHelper
    {
        // 使用 ConcurrentDictionary 缓存对象的属性信息，以提高性能
        private static readonly ConcurrentDictionary<string, PropertyInfo> CachedObjectProperties =
            new ConcurrentDictionary<string, PropertyInfo>();

        /// <summary>
        /// 尝试设置对象的属性值，使用指定的值工厂。
        /// </summary>
        /// <typeparam name="TObject">对象类型</typeparam>
        /// <typeparam name="TValue">属性值类型</typeparam>
        /// <param name="obj">要设置属性的对象</param>
        /// <param name="propertySelector">属性选择器表达式</param>
        /// <param name="valueFactory">值工厂，用于生成属性值</param>
        /// <param name="ignoreAttributeTypes">忽略的属性类型</param>
        public static void TrySetProperty<TObject, TValue>(
            TObject obj,
            Expression<Func<TObject, TValue>> propertySelector,
            Func<TValue> valueFactory,
            params Type[] ignoreAttributeTypes)
        {
            // 调用重载方法，传递对象和属性选择器表达式以及值工厂
            TrySetProperty(obj, propertySelector, x => valueFactory());
        }

        /// <summary>
        /// 尝试设置对象的属性值，使用指定的值工厂。
        /// </summary>
        /// <typeparam name="TObject">对象类型</typeparam>
        /// <typeparam name="TValue">属性值类型</typeparam>
        /// <param name="obj">要设置属性的对象</param>
        /// <param name="propertySelector">属性选择器表达式</param>
        /// <param name="valueFactory">值工厂，用于生成属性值</param>
        public static void TrySetProperty<TObject, TValue>(
            TObject obj,
            Expression<Func<TObject, TValue>> propertySelector,
            Func<TObject, TValue> valueFactory)
        {
            // 生成缓存键，格式为：对象类型全名-属性选择器表达式
            var cacheKey = $"{obj?.GetType().FullName}-{propertySelector}";

            // 从缓存中获取或添加属性信息
            var property = CachedObjectProperties.GetOrAdd(cacheKey, (c) =>
            {
                // 检查表达式类型是否为成员访问
                if (propertySelector.Body.NodeType != ExpressionType.MemberAccess)
                {
                    return null;
                }

                // 将表达式转换为成员表达式
                var memberExpression = propertySelector.Body.As<MemberExpression>();

                // 获取对象类型的所有属性
                var properties = obj?.GetType().GetProperties();

                // 查找与成员表达式匹配的属性，并且该属性具有公共的 set 方法
                var propertyInfo = properties?.FirstOrDefault(x =>
                    x.Name == memberExpression.Member.Name &&
                    x.GetSetMethod(true) != null);

                // 如果未找到匹配的属性，返回 null
                if (propertyInfo == null)
                {
                    return null;
                }

                // 返回找到的属性信息
                return propertyInfo;
            });

            // 如果属性信息不为 null，则设置属性值
            property?.SetValue(obj, valueFactory(obj));
        }
    }
}