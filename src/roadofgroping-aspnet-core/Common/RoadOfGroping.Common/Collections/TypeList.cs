namespace RoadOfGroping.Common.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// 针对 <see cref="TypeList{TBaseType}"/> 的一个快捷方式，使用对象作为基类型。
    /// </summary>
    public class TypeList : TypeList<object>, ITypeList
    {
    }

    /// <summary>
    /// 扩展 <see cref="List{Type}"/>，以添加对特定基类型的限制。
    /// </summary>
    /// <typeparam name="TBaseType">该列表中 <see cref="Type"/> 的基类型</typeparam>
    public class TypeList<TBaseType> : ITypeList<TBaseType>
    {
        private readonly List<Type> _typeList; // 存储类型的内部列表

        /// <summary>
        /// 创建新的 <see cref="TypeList{TBaseType}"/> 对象。
        /// </summary>
        public TypeList()
        {
            _typeList = new List<Type>(); // 初始化内部列表
        }

        /// <summary>
        /// 获取列表中类型的数量。
        /// </summary>
        public int Count => _typeList.Count;

        /// <summary>
        /// 获取一个值，指示此实例是否为只读。
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 获取或设置指定索引处的 <see cref="Type"/>。
        /// </summary>
        /// <param name="index">索引。</param>
        public Type this[int index]
        {
            get => _typeList[index];
            set
            {
                CheckType(value); // 检查类型是否有效
                _typeList[index] = value;
            }
        }

        /// <summary>
        /// 将类型添加到列表中。
        /// </summary>
        /// <typeparam name="T">要添加的类型</typeparam>
        public void Add<T>() where T : TBaseType
        {
            _typeList.Add(typeof(T)); // 添加类型
        }

        /// <summary>
        /// 如果类型不在列表中，则将其添加到列表。
        /// </summary>
        /// <typeparam name="T">要添加的类型</typeparam>
        public bool TryAdd<T>() where T : TBaseType
        {
            if (Contains<T>())
            {
                return false; // 类型已存在，返回 false
            }

            Add<T>(); // 否则添加类型
            return true;
        }

        /// <inheritdoc/>
        public void Add(Type item)
        {
            CheckType(item); // 检查类型是否有效
            _typeList.Add(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, Type item)
        {
            CheckType(item); // 检查类型是否有效
            _typeList.Insert(index, item);
        }

        /// <inheritdoc/>
        public int IndexOf(Type item)
        {
            return _typeList.IndexOf(item); // 返回指定类型的索引
        }

        /// <inheritdoc/>
        public bool Contains<T>() where T : TBaseType
        {
            return Contains(typeof(T)); // 检查包含的类型
        }

        /// <inheritdoc/>
        public bool Contains(Type item)
        {
            return _typeList.Contains(item); // 检查指定类型是否存在
        }

        /// <inheritdoc/>
        public void Remove<T>() where T : TBaseType
        {
            _typeList.Remove(typeof(T)); // 移除指定类型
        }

        /// <inheritdoc/>
        public bool Remove(Type item)
        {
            return _typeList.Remove(item); // 移除指定类型
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            _typeList.RemoveAt(index); // 移除指定索引的类型
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _typeList.Clear(); // 清空列表
        }

        /// <inheritdoc/>
        public void CopyTo(Type[] array, int arrayIndex)
        {
            _typeList.CopyTo(array, arrayIndex); // 将列表复制到数组
        }

        /// <inheritdoc/>
        public IEnumerator<Type> GetEnumerator()
        {
            return _typeList.GetEnumerator(); // 获取枚举器
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // 获取枚举器（非泛型）
        }

        /// <summary>
        /// 检查给定类型是否合法。
        /// </summary>
        /// <param name="item">要检查的类型</param>
        /// <exception cref="ArgumentException">如果给定类型不符合基类型的要求</exception>
        private static void CheckType(Type item)
        {
            if (!typeof(TBaseType).GetTypeInfo().IsAssignableFrom(item))
            {
                throw new ArgumentException($"给定类型 ({item.AssemblyQualifiedName}) 必须是 {typeof(TBaseType).AssemblyQualifiedName} 的实例。", nameof(item));
            }
        }
    }
}