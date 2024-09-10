namespace RoadOfGroping.Common.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using RoadOfGroping.Common.Attributes;

    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举值上的指定类型的属性。
        /// </summary>
        /// <typeparam name="T">要检索的属性的类型。</typeparam>
        /// <param name="enumValue">枚举值。</param>
        /// <returns>
        /// 指定类型的属性或 null。
        /// </returns>
        public static T GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
        {
            // 获取枚举值的类型
            var type = enumValue.GetType();

            // 获取枚举值的成员信息
            var memInfo = type.GetMember(enumValue.ToString()).FirstOrDefault();
            if (memInfo == null) return null;

            // 获取指定类型的自定义属性
            var attributes = memInfo.GetCustomAttributes<T>(false);

            // 返回第一个匹配的属性，如果没有则返回 null
            return attributes.FirstOrDefault();
        }

        /// <summary>
        /// 获取枚举值的显示名称。
        /// </summary>
        /// <param name="enumValue">枚举值。</param>
        /// <returns>
        /// 如果存在 <see cref="DisplayAttribute"/>，则使用其 Name 属性。
        /// 否则，使用标准的字符串表示。
        /// </returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            // 获取 DisplayAttribute 属性
            var attribute = enumValue.GetAttributeOfType<DisplayAttribute>();

            // 如果属性为 null，则返回枚举值的标准字符串表示，否则返回属性的 Name 属性
            return attribute == null ? enumValue.ToString() : attribute.Name;
        }

        /// <summary>
        /// 获取枚举值的名称值。
        /// </summary>
        /// <param name="value">枚举值。</param>
        /// <returns>
        /// 枚举值的名称值。
        /// </returns>
        public static string ToNameValue(this Enum value)
        {
            // 如果枚举值为 null 或其字符串表示为空，则返回 null
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            // 获取枚举值的成员信息
            var memberInfo = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                // 调用成员信息的 ToNameValue 方法
                return memberInfo.ToNameValue();
            }

            // 如果没有成员信息，则返回枚举值的标准字符串表示
            return value.ToString();
        }

        /// <summary>
        /// 获取成员信息的名称值。
        /// </summary>
        /// <param name="member">成员信息。</param>
        /// <returns>
        /// 成员信息的名称值。
        /// </returns>
        public static string ToNameValue(this MemberInfo member)
        {
            // 获取 EnumNameAttribute 属性
            var attribute = member.GetAttribute<EnumNameAttribute>();
            if (attribute == null)
            {
                // 如果属性为 null，则返回成员信息的名称
                return member.Name;
            }

            // 返回属性的 NameValue 属性
            return attribute.NameValue;
        }

        /// <summary>
        /// 获取成员信息上的指定类型的属性。
        /// </summary>
        /// <typeparam name="T">要检索的属性的类型。</typeparam>
        /// <param name="member">成员信息。</param>
        /// <returns>
        /// 指定类型的属性或 null。
        /// </returns>
        private static T GetAttribute<T>(this MemberInfo member) where T : Attribute
        {
            // 获取指定类型的自定义属性
            var attributes = member.GetCustomAttributes<T>(false);

            // 返回第一个匹配的属性，如果没有则返回 null
            return attributes.FirstOrDefault();
        }
    }
}