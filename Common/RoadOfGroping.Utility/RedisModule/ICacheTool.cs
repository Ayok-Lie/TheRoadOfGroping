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
}