namespace RoadOfGroping.Common.Collections
{
    /// <summary>
    /// 针对 <see cref="ITypeList{TBaseType}"/> 的一个快捷方式，使用对象作为基类型。
    /// </summary>
    public interface ITypeList : ITypeList<object>
    {
    }

    /// <summary>
    /// 扩展 <see cref="IList{Type}"/>，以添加对特定基类型的限制。
    /// </summary>
    /// <typeparam name="TBaseType">该列表中 <see cref="Type"/> 的基类型</typeparam>
    public interface ITypeList<in TBaseType> : IList<Type>
    {
        /// <summary>
        /// 将类型添加到列表中。
        /// </summary>
        /// <typeparam name="T">要添加的类型</typeparam>
        void Add<T>() where T : TBaseType;

        /// <summary>
        /// 如果类型不在列表中，则将该类型添加到列表中。
        /// </summary>
        /// <typeparam name="T">要添加的类型</typeparam>
        /// <returns>返回是否成功添加</returns>
        bool TryAdd<T>() where T : TBaseType;

        /// <summary>
        /// 检查类型是否存在于列表中。
        /// </summary>
        /// <typeparam name="T">要检查的类型</typeparam>
        /// <returns>如果存在，返回真；否则返回假</returns>
        bool Contains<T>() where T : TBaseType;

        /// <summary>
        /// 从列表中移除指定类型。
        /// </summary>
        /// <typeparam name="T">要移除的类型</typeparam>
        void Remove<T>() where T : TBaseType;
    }
}