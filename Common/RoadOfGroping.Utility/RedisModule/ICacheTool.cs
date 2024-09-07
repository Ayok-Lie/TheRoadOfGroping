using FreeRedis;

public interface ICacheTool
{
    /// <summary>
    /// 删除指定的键
    /// </summary>
    /// <param name="key">要删除的键</param>
    /// <returns>删除的键数量</returns>
    long Del(params string[] key);

    /// <summary>
    /// 异步删除指定的键
    /// </summary>
    /// <param name="key">要删除的键</param>
    /// <returns>删除的键数量</returns>
    Task<long> DelAsync(params string[] key);

    /// <summary>
    /// 检查键是否存在
    /// </summary>
    /// <param name="key">要检查的键</param>
    /// <returns>如果键存在则返回 true，否则返回 false</returns>
    bool Exists(string key);

    /// <summary>
    /// 异步检查键是否存在
    /// </summary>
    /// <param name="key">要检查的键</param>
    /// <returns>如果键存在则返回 true，否则返回 false</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 获取键的值
    /// </summary>
    /// <param name="key">要获取值的键</param>
    /// <returns>键的值</returns>
    string Get(string key);

    /// <summary>
    /// 获取键的值并转换为指定类型
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="key">要获取值的键</param>
    /// <returns>键的值</returns>
    T Get<T>(string key);

    /// <summary>
    /// 异步获取键的值
    /// </summary>
    /// <param name="key">要获取值的键</param>
    /// <returns>键的值</returns>
    Task<string> GetAsync(string key);

    /// <summary>
    /// 异步获取键的值并转换为指定类型
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="key">要获取值的键</param>
    /// <returns>键的值</returns>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// 设置键的值
    /// </summary>
    /// <param name="key">要设置值的键</param>
    /// <param name="value">值</param>
    void Set(string key, object value);

    /// <summary>
    /// 设置键的值并设置过期时间
    /// </summary>
    /// <param name="key">要设置值的键</param>
    /// <param name="value">值</param>
    /// <param name="expire">过期时间</param>
    void Set(string key, object value, TimeSpan expire);

    /// <summary>
    /// 异步设置键的值
    /// </summary>
    /// <param name="key">要设置值的键</param>
    /// <param name="value">值</param>
    /// <param name="expire">过期时间</param>
    Task SetAsync(string key, object value, TimeSpan? expire = null);

    /// <summary>
    /// 异步获取键的值，如果不存在则设置新值
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="key">要获取或设置值的键</param>
    /// <param name="func">获取新值的函数</param>
    /// <param name="expire">过期时间</param>
    /// <returns>键的值</returns>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expire = null);

    /// <summary>
    /// 模糊匹配删除
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task RemoveByPrefixAsync(string key);

    #region 列表（List）

    /// <summary>
    /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    string BLPop(string key, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    T BLPop<T>(string key, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="keys">键数组</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns><param name="timeoutSeconds">超时时间（秒）</param>
    KeyValue<string> BLPop(string[] keys, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys">键数组</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    KeyValue<T> BLPop<T>(string[] keys, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    string BRPop(string key, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    T BRPop<T>(string key, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="keys">键数组</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    KeyValue<string> BRPop(string[] keys, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys">键数组</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    KeyValue<T> BRPop<T>(string[] keys, int timeoutSeconds);

    /// <summary>
    /// 从列表中取出最后一个元素，并插入到另外一个列表的头部（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="source">源列表</param>
    /// <param name="destination">目标列表</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    string BRPopLPush(string source, string destination, int timeoutSeconds);

    /// <summary>
    /// 从列表中取出最后一个元素，并插入到另外一个列表的头部（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">源列表</param>
    /// <param name="destination">目标列表</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    T BRPopLPush<T>(string source, string destination, int timeoutSeconds);

    /// <summary>
    /// 通过索引获取列表中的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index">索引</param>
    /// <returns></returns>
    string LIndex(string key, long index);

    /// <summary>
    /// 通过索引获取列表中的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="index">索引</param>
    /// <returns></returns>
    T LIndex<T>(string key, long index);

    /// <summary>
    /// 指定列表中一个元素在它之前或之后插入另外一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="direction">插入方向（before|after）</param>
    /// <param name="pivot">参照元素</param>
    /// <param name="element">待插入的元素</param>
    /// <returns></returns>
    long LInsert(string key, InsertDirection direction, object pivot, object element);

    /// <summary>
    /// 获取列表的长度
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    long LLen(string key);

    /// <summary>
    /// 从列表的头部弹出元素，默认为第一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string LPop(string key);

    /// <summary>
    /// 从列表的头部弹出元素，默认为第一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T LPop<T>(string key);

    /// <summary>
    /// 获取列表 key 中匹配给定 element 成员的索引
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="element">元素</param>
    /// <param name="rank">从第几个匹配开始计算</param>
    /// <returns></returns>
    long LPos<T>(string key, T element, int rank = 0);

    /// <summary>
    /// 获取列表 key 中匹配给定 element 成员的索引
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="element">元素</param>
    /// <param name="rank">从第几个匹配开始计算</param>
    /// <param name="count">要匹配的总数</param>
    /// <param name="maxLen">只查找最多 len 个成员</param>
    /// <returns></returns>
    long[] LPos<T>(string key, T element, int rank, int count, int maxLen);

    /// <summary>
    /// 在列表头部插入一个或者多个值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素数组</param>
    /// <returns></returns>
    long LPush(string key, params object[] elements);

    /// <summary>
    /// 当储存列表的 key 存在时，用于将值插入到列表头部
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素数组</param>
    /// <returns></returns>
    long LPushX(string key, params object[] elements);

    /// <summary>
    /// 获取列表中指定区间内的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">开始偏移量</param>
    /// <param name="stop">结束偏移量</param>
    /// <returns></returns>
    string[] LRange(string key, long start, long stop);

    /// <summary>
    /// 获取列表中指定区间内的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="start">开始偏移量</param>
    /// <param name="stop">结束偏移量</param>
    /// <returns></returns>
    T[] LRange<T>(string key, long start, long stop);

    /// <summary>
    /// 从列表中删除元素与 value 相等的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="count">删除的数量（等于0时全部移除，小于0时从表尾开始向表头搜索，大于0时从表头开始向表尾搜索）</param>
    /// <param name="element">待删除的元素</param>
    /// <returns></returns>
    long LRem<T>(string key, long count, T element);

    /// <summary>
    /// 通过其索引设置列表中元素的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="index">索引</param>
    /// <param name="element">元素</param>
    void LSet<T>(string key, long index, T element);

    /// <summary>
    /// 保留列表中指定范围内的元素值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">开始偏移量</param>
    /// <param name="stop">结束偏移量</param>
    void LTrim(string key, long start, long stop);

    /// <summary>
    /// 移除列表的最后一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string RPop(string key);

    /// <summary>
    /// 移除列表的最后一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T RPop<T>(string key);

    /// <summary>
    /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
    /// </summary>
    /// <param name="source">源列表</param>
    /// <param name="destination">目标列表</param>
    /// <returns></returns>
    string RPopLPush(string source, string destination);

    /// <summary>
    /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">源列表</param>
    /// <param name="destination">目标列表</param>
    /// <returns></returns>
    T RPopLPush<T>(string source, string destination);

    /// <summary>
    /// 在列表中添加一个或多个值到列表尾部
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素数组</param>
    /// <returns></returns>
    long RPush(string key, params object[] elements);

    /// <summary>
    /// 为已存在的列表添加值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素数组</param>
    /// <returns></returns>
    long RPushX(string key, params object[] elements);

    /// <summary>
    /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<string> BLPopAsync(string key, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<T> BLPopAsync<T>(string key, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="keys">键数组</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns><param name="timeoutSeconds">超时时间（秒）</param>
    Task<KeyValue<string>> BLPopAsync(string[] keys, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys">键数组</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<KeyValue<T>> BLPopAsync<T>(string[] keys, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<string> BRPopAsync(string key, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<T> BRPopAsync<T>(string key, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="keys">键数组</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<KeyValue<string>> BRPopAsync(string[] keys, int timeoutSeconds);

    /// <summary>
    /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys">键数组</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<KeyValue<T>> BRPopAsync<T>(string[] keys, int timeoutSeconds);

    /// <summary>
    /// 从列表中取出最后一个元素，并插入到另外一个列表的头部（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <param name="source">源列表</param>
    /// <param name="destination">目标列表</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<string> BRPopLPushAsync(string source, string destination, int timeoutSeconds);

    /// <summary>
    /// 从列表中取出最后一个元素，并插入到另外一个列表的头部（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">源列表</param>
    /// <param name="destination">目标列表</param>
    /// <param name="timeoutSeconds">超时时间（秒）</param>
    /// <returns></returns>
    Task<T> BRPopLPushAsync<T>(string source, string destination, int timeoutSeconds);

    /// <summary>
    /// 通过索引获取列表中的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index">索引</param>
    /// <returns></returns>
    Task<string> LIndexAsync(string key, long index);

    /// <summary>
    /// 通过索引获取列表中的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="index">索引</param>
    /// <returns></returns>
    Task<T> LIndexAsync<T>(string key, long index);

    /// <summary>
    /// 指定列表中一个元素在它之前或之后插入另外一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="direction">插入方向（before|after）</param>
    /// <param name="pivot">参照元素</param>
    /// <param name="element">待插入的元素</param>
    /// <returns></returns>
    Task<long> LInsertAsync(string key, InsertDirection direction, object pivot, object element);

    /// <summary>
    /// 获取列表的长度
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<long> LLenAsync(string key);

    /// <summary>
    /// 从列表的头部弹出元素，默认为第一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string> LPopAsync(string key);

    /// <summary>
    /// 从列表的头部弹出元素，默认为第一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T> LPopAsync<T>(string key);

    /// <summary>
    /// 获取列表 key 中匹配给定 element 成员的索引
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="element">元素</param>
    /// <param name="rank">从第几个匹配开始计算</param>
    /// <returns></returns>
    Task<long> LPosAsync<T>(string key, T element, int rank = 0);

    /// <summary>
    /// 获取列表 key 中匹配给定 element 成员的索引
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="element">元素</param>
    /// <param name="rank">从第几个匹配开始计算</param>
    /// <param name="count">要匹配的总数</param>
    /// <param name="maxLen">只查找最多 len 个成员</param>
    /// <returns></returns>
    Task<long[]> LPosAsync<T>(string key, T element, int rank, int count, int maxLen);

    /// <summary>
    /// 在列表头部插入一个或者多个值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素数组</param>
    /// <returns></returns>
    Task<long> LPushAsync(string key, params object[] elements);

    /// <summary>
    /// 当储存列表的 key 存在时，用于将值插入到列表头部
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素数组</param>
    /// <returns></returns>
    Task<long> LPushXAsync(string key, params object[] elements);

    /// <summary>
    /// 获取列表中指定区间内的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">开始偏移量</param>
    /// <param name="stop">结束偏移量</param>
    /// <returns></returns>
    Task<string[]> LRangeAsync(string key, long start, long stop);

    /// <summary>
    /// 获取列表中指定区间内的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="start">开始偏移量</param>
    /// <param name="stop">结束偏移量</param>
    /// <returns></returns>
    Task<T[]> LRangeAsync<T>(string key, long start, long stop);

    /// <summary>
    /// 从列表中删除元素与 value 相等的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="count">删除的数量（等于0时全部移除，小于0时从表尾开始向表头搜索，大于0时从表头开始向表尾搜索）</param>
    /// <param name="element">待删除的元素</param>
    /// <returns></returns>
    Task<long> LRemAsync<T>(string key, long count, T element);

    /// <summary>
    /// 通过其索引设置列表中元素的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="index">索引</param>
    /// <param name="element">元素</param>
    Task LSetAsync<T>(string key, long index, T element);

    /// <summary>
    /// 保留列表中指定范围内的元素值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">开始偏移量</param>
    /// <param name="stop">结束偏移量</param>
    Task LTrimAsync(string key, long start, long stop);

    /// <summary>
    /// 移除列表的最后一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string> RPopAsync(string key);

    /// <summary>
    /// 移除列表的最后一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T> RPopAsync<T>(string key);

    /// <summary>
    /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
    /// </summary>
    /// <param name="source">源列表</param>
    /// <param name="destination">目标列表</param>
    /// <returns></returns>
    Task<string> RPopLPushAsync(string source, string destination);

    /// <summary>
    /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">源列表</param>
    /// <param name="destination">目标列表</param>
    /// <returns></returns>
    Task<T> RPopLPushAsync<T>(string source, string destination);

    /// <summary>
    /// 在列表中添加一个或多个值到列表尾部
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素数组</param>
    /// <returns></returns>
    Task<long> RPushAsync(string key, params object[] elements);

    /// <summary>
    /// 为已存在的列表添加值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素数组</param>
    /// <returns></returns>
    Task<long> RPushXAsync(string key, params object[] elements);

    #endregion 列表（List）
}