using FreeRedis;
using RoadOfGroping.Common.Extensions;

namespace RoadOfGroping.Utility.RedisModule
{
    public class RedisCacheTool : ICacheTool
    {
        private readonly RedisClient _redisClient;

        /// <summary>
        /// 构造函数，初始化 RedisClient 实例
        /// </summary>
        /// <param name="redisClient">RedisClient 实例</param>
        public RedisCacheTool(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 创建缓存Key
        /// </summary>
        /// <param name="idKey">缓存键的一部分</param>
        /// <returns>完整的缓存键</returns>
        protected string BuildKey(string idKey)
        {
            return $"Cache_{GetType().FullName}_{idKey}";
        }

        /// <summary>
        /// 创建多个缓存Key
        /// </summary>
        /// <param name="keys">多个缓存键的一部分</param>
        /// <returns>完整的缓存键数组</returns>
        protected string[] BuildKey(params string[] keys)
        {
            foreach (var item in keys)
            {
                BuildKey(item);
            }
            return keys;
        }

        /// <summary>
        /// 删除一个或多个缓存项
        /// </summary>
        /// <param name="key">一个或多个缓存键</param>
        /// <returns>删除的缓存项数量</returns>
        public long Del(params string[] key)
        {
            return _redisClient.Del(BuildKey(key));
        }

        /// <summary>
        /// 异步删除一个或多个缓存项
        /// </summary>
        /// <param name="key">一个或多个缓存键</param>
        /// <returns>删除的缓存项数量</returns>
        public Task<long> DelAsync(params string[] key)
        {
            return _redisClient.DelAsync(BuildKey(key));
        }

        /// <summary>
        /// 检查缓存项是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>如果存在返回 true，否则返回 false</returns>
        public bool Exists(string key)
        {
            return _redisClient.Exists(BuildKey(key));
        }

        /// <summary>
        /// 异步检查缓存项是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>如果存在返回 true，否则返回 false</returns>
        public Task<bool> ExistsAsync(string key)
        {
            return _redisClient.ExistsAsync(BuildKey(key));
        }

        /// <summary>
        /// 获取缓存项的字符串值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存项的字符串值</returns>
        public string Get(string key)
        {
            return _redisClient.Get(BuildKey(key));
        }

        /// <summary>
        /// 获取缓存项的泛型值
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存项的泛型值</returns>
        public T Get<T>(string key)
        {
            return _redisClient.Get<T>(BuildKey(key));
        }

        /// <summary>
        /// 异步获取缓存项的字符串值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存项的字符串值</returns>
        public Task<string> GetAsync(string key)
        {
            return _redisClient.GetAsync(BuildKey(key));
        }

        /// <summary>
        /// 异步获取缓存项的泛型值
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存项的泛型值</returns>
        public Task<T> GetAsync<T>(string key)
        {
            return _redisClient.GetAsync<T>(BuildKey(key));
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        public void Set(string key, object value)
        {
            string cacheKey = BuildKey(key);
            _redisClient.Set(cacheKey, value.ToJson());
        }

        /// <summary>
        /// 设置带有过期时间的缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expire">过期时间</param>
        public void Set(string key, object value, TimeSpan expire)
        {
            string cacheKey = BuildKey(key);
            _redisClient.Set(cacheKey, value.ToJson(), expire);
        }

        /// <summary>
        /// 异步设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expire">过期时间</param>
        /// <returns>异步任务</returns>
        public Task SetAsync(string key, object value, TimeSpan? expire = null)
        {
            string cacheKey = BuildKey(key);
            return _redisClient.SetAsync(cacheKey, value, GetExpireSeconds(expire));
        }

        /// <summary>
        /// 获取过期时间的秒数
        /// </summary>
        /// <param name="expire">过期时间</param>
        /// <returns>过期时间的秒数</returns>
        private int GetExpireSeconds(TimeSpan? expire)
        {
            return expire.HasValue ? (int)expire.Value.TotalSeconds : 0;
        }

        /// <summary>
        /// 异步获取或设置缓存项
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="func">获取缓存值的函数</param>
        /// <param name="expire">过期时间</param>
        /// <returns>缓存项的泛型值</returns>
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expire = null)
        {
            if (await _redisClient.ExistsAsync(key))
            {
                try
                {
                    return await _redisClient.GetAsync<T>(key);
                }
                catch (Exception ex)
                {
                    // 记录异常信息
                    Console.WriteLine($"Error getting key {key}: {ex.Message}");
                    await _redisClient.DelAsync(key);
                }
            }

            var result = await func.Invoke();
            await _redisClient.SetAsync(key, result, GetExpireSeconds(expire));

            return result;
        }

        /// <summary>
        /// 异步删除前缀匹配的缓存项
        /// </summary>
        /// <param name="key">缓存键前缀</param>
        /// <returns>异步任务</returns>
        public async Task RemoveByPrefixAsync(string key)
        {
            var keys = await _redisClient.KeysAsync("*" + BuildKey(key) + "*");
            foreach (var item in keys)
            {
                await _redisClient.DelAsync(item);
            }
        }

        #region 列表（List）

        /// <summary>
        /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public string BLPop(string key, int timeoutSeconds) => _redisClient.BLPop(key, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public T BLPop<T>(string key, int timeoutSeconds) => _redisClient.BLPop<T>(key, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns><param name="timeoutSeconds">超时时间（秒）</param>
        public KeyValue<string> BLPop(string[] keys, int timeoutSeconds) => _redisClient.BLPop(keys, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">键数组</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public KeyValue<T> BLPop<T>(string[] keys, int timeoutSeconds) => _redisClient.BLPop<T>(keys, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public string BRPop(string key, int timeoutSeconds) => _redisClient.BRPop(key, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public T BRPop<T>(string key, int timeoutSeconds) => _redisClient.BRPop<T>(key, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public KeyValue<string> BRPop(string[] keys, int timeoutSeconds) => _redisClient.BRPop(keys, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">键数组</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public KeyValue<T> BRPop<T>(string[] keys, int timeoutSeconds) => _redisClient.BRPop<T>(keys, timeoutSeconds);

        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public string BRPopLPush(string source, string destination, int timeoutSeconds) => _redisClient.BRPopLPush(source, destination, timeoutSeconds);

        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public T BRPopLPush<T>(string source, string destination, int timeoutSeconds) => _redisClient.BRPopLPush<T>(source, destination, timeoutSeconds);

        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public string LIndex(string key, long index) => _redisClient.LIndex(key, index);

        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public T LIndex<T>(string key, long index) => _redisClient.LIndex<T>(key, index);

        /// <summary>
        /// 指定列表中一个元素在它之前或之后插入另外一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="direction">插入方向（before|after）</param>
        /// <param name="pivot">参照元素</param>
        /// <param name="element">待插入的元素</param>
        /// <returns></returns>
        public long LInsert(string key, InsertDirection direction, object pivot, object element) => _redisClient.LInsert(key, direction, pivot, element);

        /// <summary>
        /// 获取列表的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long LLen(string key) => _redisClient.LLen(key);

        /// <summary>
        /// 从列表的头部弹出元素，默认为第一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string LPop(string key) => _redisClient.LPop(key);

        /// <summary>
        /// 从列表的头部弹出元素，默认为第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T LPop<T>(string key) => _redisClient.LPop<T>(key);

        /// <summary>
        /// 获取列表 key 中匹配给定 element 元素的索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="element">元素</param>
        /// <param name="rank">从第几个匹配开始计算</param>
        /// <returns></returns>
        public long LPos<T>(string key, T element, int rank = 0) => _redisClient.LPos<T>(key, element, rank);

        /// <summary>
        /// 获取列表 key 中匹配给定 element 元素的索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="element">元素</param>
        /// <param name="rank">从第几个匹配开始计算</param>
        /// <param name="count">要匹配的总数</param>
        /// <param name="maxLen">只查找最多 len 个元素</param>
        /// <returns></returns>
        public long[] LPos<T>(string key, T element, int rank, int count, int maxLen) => _redisClient.LPos<T>(key, element, rank, count, maxLen);

        /// <summary>
        /// 在列表头部插入一个或者多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elements">元素数组</param>
        /// <returns></returns>
        public long LPush(string key, params object[] elements) => _redisClient.LPush(key, elements);

        /// <summary>
        /// 当储存列表的 key 存在时，用于将值插入到列表头部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elements">元素数组</param>
        /// <returns></returns>
        public long LPushX(string key, params object[] elements) => _redisClient.LPushX(key, elements);

        /// <summary>
        /// 获取列表中指定区间内的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始偏移量</param>
        /// <param name="stop">结束偏移量</param>
        /// <returns></returns>
        public string[] LRange(string key, long start, long stop) => _redisClient.LRange(key, start, stop);

        /// <summary>
        /// 获取列表中指定区间内的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start">开始偏移量</param>
        /// <param name="stop">结束偏移量</param>
        /// <returns></returns>
        public T[] LRange<T>(string key, long start, long stop) => _redisClient.LRange<T>(key, start, stop);

        /// <summary>
        /// 从列表中删除元素与 value 相等的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="count">删除的数量（等于0时全部移除，小于0时从表尾开始向表头搜索，大于0时从表头开始向表尾搜索）</param>
        /// <param name="element">待删除的元素</param>
        /// <returns></returns>
        public long LRem<T>(string key, long count, T element) => _redisClient.LRem<T>(key, count, element);

        /// <summary>
        /// 通过其索引设置列表中元素的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <param name="element">元素</param>
        public void LSet<T>(string key, long index, T element) => _redisClient.LSet<T>(key, index, element);

        /// <summary>
        /// 保留列表中指定范围内的元素值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始偏移量</param>
        /// <param name="stop">结束偏移量</param>
        public void LTrim(string key, long start, long stop) => _redisClient.LTrim(key, start, stop);

        /// <summary>
        /// 移除列表的最后一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string RPop(string key) => _redisClient.RPop(key);

        /// <summary>
        /// 移除列表的最后一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T RPop<T>(string key) => _redisClient.RPop<T>(key);

        /// <summary>
        /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
        /// </summary>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <returns></returns>
        public string RPopLPush(string source, string destination) => _redisClient.RPopLPush(source, destination);

        /// <summary>
        /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <returns></returns>
        public T RPopLPush<T>(string source, string destination) => _redisClient.RPopLPush<T>(source, destination);

        /// <summary>
        /// 在列表中添加一个或多个值到列表尾部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elements">元素数组</param>
        /// <returns></returns>
        public long RPush(string key, params object[] elements) => _redisClient.RPush(key, elements);

        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elements">元素数组</param>
        /// <returns></returns>
        public long RPushX(string key, params object[] elements) => _redisClient.RPushX(key, elements);

        /// <summary>
        /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<string> BLPopAsync(string key, int timeoutSeconds) => await _redisClient.BLPopAsync(key, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<T> BLPopAsync<T>(string key, int timeoutSeconds) => await _redisClient.BLPopAsync<T>(key, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns><param name="timeoutSeconds">超时时间（秒）</param>
        public async Task<KeyValue<string>> BLPopAsync(string[] keys, int timeoutSeconds) => await _redisClient.BLPopAsync(keys, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的第一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">键数组</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<KeyValue<T>> BLPopAsync<T>(string[] keys, int timeoutSeconds) => await _redisClient.BLPopAsync<T>(keys, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<string> BRPopAsync(string key, int timeoutSeconds) => await _redisClient.BRPopAsync(key, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<T> BRPopAsync<T>(string key, int timeoutSeconds) => await _redisClient.BRPopAsync<T>(key, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<KeyValue<string>> BRPopAsync(string[] keys, int timeoutSeconds) => await _redisClient.BRPopAsync(keys, timeoutSeconds);

        /// <summary>
        /// 移出并获取列表的最后一个元素（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">键数组</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<KeyValue<T>> BRPopAsync<T>(string[] keys, int timeoutSeconds) => await _redisClient.BRPopAsync<T>(keys, timeoutSeconds);

        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<string> BRPopLPushAsync(string source, string destination, int timeoutSeconds) => await _redisClient.BRPopLPushAsync(source, destination, timeoutSeconds);

        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部（如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns></returns>
        public async Task<T> BRPopLPushAsync<T>(string source, string destination, int timeoutSeconds) => await _redisClient.BRPopLPushAsync<T>(source, destination, timeoutSeconds);

        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public async Task<string> LIndexAsync(string key, long index) => await _redisClient.LIndexAsync(key, index);

        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public async Task<T> LIndexAsync<T>(string key, long index) => await _redisClient.LIndexAsync<T>(key, index);

        /// <summary>
        /// 指定列表中一个元素在它之前或之后插入另外一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="direction">插入方向（before|after）</param>
        /// <param name="pivot">参照元素</param>
        /// <param name="element">待插入的元素</param>
        /// <returns></returns>
        public async Task<long> LInsertAsync(string key, InsertDirection direction, object pivot, object element) => await _redisClient.LInsertAsync(key, direction, pivot, element);

        /// <summary>
        /// 获取列表的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> LLenAsync(string key) => await _redisClient.LLenAsync(key);

        /// <summary>
        /// 从列表的头部弹出元素，默认为第一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> LPopAsync(string key) => await _redisClient.LPopAsync(key);

        /// <summary>
        /// 从列表的头部弹出元素，默认为第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> LPopAsync<T>(string key) => await _redisClient.LPopAsync<T>(key);

        /// <summary>
        /// 获取列表 key 中匹配给定 element 元素的索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="element">元素</param>
        /// <param name="rank">从第几个匹配开始计算</param>
        /// <returns></returns>
        public async Task<long> LPosAsync<T>(string key, T element, int rank = 0) => await _redisClient.LPosAsync<T>(key, element, rank);

        /// <summary>
        /// 获取列表 key 中匹配给定 element 元素的索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="element">元素</param>
        /// <param name="rank">从第几个匹配开始计算</param>
        /// <param name="count">要匹配的总数</param>
        /// <param name="maxLen">只查找最多 len 个元素</param>
        /// <returns></returns>
        public async Task<long[]> LPosAsync<T>(string key, T element, int rank, int count, int maxLen) => await _redisClient.LPosAsync<T>(key, element, rank, count, maxLen);

        /// <summary>
        /// 在列表头部插入一个或者多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elements">元素数组</param>
        /// <returns></returns>
        public async Task<long> LPushAsync(string key, params object[] elements) => await _redisClient.LPushAsync(key, elements);

        /// <summary>
        /// 当储存列表的 key 存在时，用于将值插入到列表头部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elements">元素数组</param>
        /// <returns></returns>
        public async Task<long> LPushXAsync(string key, params object[] elements) => await _redisClient.LPushXAsync(key, elements);

        /// <summary>
        /// 获取列表中指定区间内的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始偏移量</param>
        /// <param name="stop">结束偏移量</param>
        /// <returns></returns>
        public async Task<string[]> LRangeAsync(string key, long start, long stop) => await _redisClient.LRangeAsync(key, start, stop);

        /// <summary>
        /// 获取列表中指定区间内的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start">开始偏移量</param>
        /// <param name="stop">结束偏移量</param>
        /// <returns></returns>
        public async Task<T[]> LRangeAsync<T>(string key, long start, long stop) => await _redisClient.LRangeAsync<T>(key, start, stop);

        /// <summary>
        /// 从列表中删除元素与 value 相等的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="count">删除的数量（等于0时全部移除，小于0时从表尾开始向表头搜索，大于0时从表头开始向表尾搜索）</param>
        /// <param name="element">待删除的元素</param>
        /// <returns></returns>
        public async Task<long> LRemAsync<T>(string key, long count, T element) => await _redisClient.LRemAsync<T>(key, count, element);

        /// <summary>
        /// 通过其索引设置列表中元素的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <param name="element">元素</param>
        public async Task LSetAsync<T>(string key, long index, T element) => await _redisClient.LSetAsync<T>(key, index, element);

        /// <summary>
        /// 保留列表中指定范围内的元素值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始偏移量</param>
        /// <param name="stop">结束偏移量</param>
        public async Task LTrimAsync(string key, long start, long stop) => await _redisClient.LTrimAsync(key, start, stop);

        /// <summary>
        /// 移除列表的最后一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> RPopAsync(string key) => await _redisClient.RPopAsync(key);

        /// <summary>
        /// 移除列表的最后一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> RPopAsync<T>(string key) => await _redisClient.RPopAsync<T>(key);

        /// <summary>
        /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
        /// </summary>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <returns></returns>
        public async Task<string> RPopLPushAsync(string source, string destination) => await _redisClient.RPopLPushAsync(source, destination);

        /// <summary>
        /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <returns></returns>
        public async Task<T> RPopLPushAsync<T>(string source, string destination) => await _redisClient.RPopLPushAsync<T>(source, destination);

        /// <summary>
        /// 在列表中添加一个或多个值到列表尾部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elements">元素数组</param>
        /// <returns></returns>
        public async Task<long> RPushAsync(string key, params object[] elements) => await _redisClient.RPushAsync(key, elements);

        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="elements">元素数组</param>
        /// <returns></returns>
        public async Task<long> RPushXAsync(string key, params object[] elements) => await _redisClient.RPushXAsync(key, elements);

        #endregion 列表（List）
    }
}