using FreeRedis;
using Microsoft.Extensions.Caching.Memory;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.RedisModule
{
    public class MemoryCacheTool : ICacheTool
    {
        // 内存缓存实例
        private readonly IMemoryCache _memoryCache;

        // 构造函数，初始化内存缓存实例
        public MemoryCacheTool(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // 删除一个或多个缓存项
        public long Del(params string[] key)
        {
            foreach (var k in key)
            {
                _memoryCache.Remove(k);
            }
            return key.Length;
        }

        // 异步删除一个或多个缓存项
        public Task<long> DelAsync(params string[] key)
        {
            foreach (var k in key)
            {
                _memoryCache.Remove(k);
            }
            return Task.FromResult((long)key.Length);
        }

        // 检查缓存项是否存在
        public bool Exists(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        // 异步检查缓存项是否存在
        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        // 获取缓存项的字符串值
        public string Get(string key)
        {
            return _memoryCache.Get(key)?.ToString();
        }

        // 获取缓存项的泛型值
        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        // 异步获取缓存项的字符串值
        public Task<string> GetAsync(string key)
        {
            return Task.FromResult(Get(key));
        }

        // 异步获取缓存项的泛型值
        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(Get<T>(key));
        }

        // 设置缓存项
        public void Set(string key, object value)
        {
            if (value == null)
            {
                throw new Exception("不能将 NULL 插入缓存。");
            }
            _memoryCache.Set(key, value);
        }

        // 设置带有过期时间的缓存项
        public void Set(string key, object value, TimeSpan expire)
        {
            if (value == null)
            {
                throw new Exception("不能将 NULL 插入缓存。");
            }
            _memoryCache.Set(key, value, expire);
        }

        // 异步设置缓存项
        public Task SetAsync(string key, object value, TimeSpan? expire = null)
        {
            if (value == null)
            {
                throw new Exception("不能将 NULL 插入缓存。");
            }

            if (expire.HasValue)
            {
                Set(key, value, expire.Value);
            }
            else
            {
                Set(key, value);
            }
            return Task.CompletedTask;
        }

        // 异步获取或设置缓存项
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expire = null)
        {
            if (await ExistsAsync(key))
            {
                try
                {
                    return await GetAsync<T>(key);
                }
                catch (Exception ex)
                {
                    // 记录异常信息
                    Console.WriteLine($"Error getting key {key}: {ex.Message}");
                    await DelAsync(key);
                }
            }

            // 调用传入的函数获取值
            var result = await func.Invoke();

            if (expire.HasValue)
            {
                await SetAsync(key, result, expire.Value);
            }
            else
            {
                await SetAsync(key, result);
            }

            return result;
        }

        // 异步删除前缀匹配的缓存项
        public Task RemoveByPrefixAsync(string key)
        {
            throw new NotImplementedException();
        }

        public string BLPop(string key, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public T BLPop<T>(string key, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public KeyValue<string> BLPop(string[] keys, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public KeyValue<T> BLPop<T>(string[] keys, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public string BRPop(string key, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public T BRPop<T>(string key, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public KeyValue<string> BRPop(string[] keys, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public KeyValue<T> BRPop<T>(string[] keys, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public string BRPopLPush(string source, string destination, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public T BRPopLPush<T>(string source, string destination, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public string LIndex(string key, long index)
        {
            throw new NotImplementedException();
        }

        public T LIndex<T>(string key, long index)
        {
            throw new NotImplementedException();
        }

        public long LInsert(string key, InsertDirection direction, object pivot, object element)
        {
            throw new NotImplementedException();
        }

        public long LLen(string key)
        {
            throw new NotImplementedException();
        }

        public string LPop(string key)
        {
            throw new NotImplementedException();
        }

        public T LPop<T>(string key)
        {
            throw new NotImplementedException();
        }

        public long LPos<T>(string key, T element, int rank = 0)
        {
            throw new NotImplementedException();
        }

        public long[] LPos<T>(string key, T element, int rank, int count, int maxLen)
        {
            throw new NotImplementedException();
        }

        public long LPush(string key, params object[] elements)
        {
            throw new NotImplementedException();
        }

        public long LPushX(string key, params object[] elements)
        {
            throw new NotImplementedException();
        }

        public string[] LRange(string key, long start, long stop)
        {
            throw new NotImplementedException();
        }

        public T[] LRange<T>(string key, long start, long stop)
        {
            throw new NotImplementedException();
        }

        public long LRem<T>(string key, long count, T element)
        {
            throw new NotImplementedException();
        }

        public void LSet<T>(string key, long index, T element)
        {
            throw new NotImplementedException();
        }

        public void LTrim(string key, long start, long stop)
        {
            throw new NotImplementedException();
        }

        public string RPop(string key)
        {
            throw new NotImplementedException();
        }

        public T RPop<T>(string key)
        {
            throw new NotImplementedException();
        }

        public string RPopLPush(string source, string destination)
        {
            throw new NotImplementedException();
        }

        public T RPopLPush<T>(string source, string destination)
        {
            throw new NotImplementedException();
        }

        public long RPush(string key, params object[] elements)
        {
            throw new NotImplementedException();
        }

        public long RPushX(string key, params object[] elements)
        {
            throw new NotImplementedException();
        }

        public Task<string> BLPopAsync(string key, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<T> BLPopAsync<T>(string key, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValue<string>> BLPopAsync(string[] keys, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValue<T>> BLPopAsync<T>(string[] keys, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<string> BRPopAsync(string key, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<T> BRPopAsync<T>(string key, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValue<string>> BRPopAsync(string[] keys, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValue<T>> BRPopAsync<T>(string[] keys, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<string> BRPopLPushAsync(string source, string destination, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<T> BRPopLPushAsync<T>(string source, string destination, int timeoutSeconds)
        {
            throw new NotImplementedException();
        }

        public Task<string> LIndexAsync(string key, long index)
        {
            throw new NotImplementedException();
        }

        public Task<T> LIndexAsync<T>(string key, long index)
        {
            throw new NotImplementedException();
        }

        public Task<long> LInsertAsync(string key, InsertDirection direction, object pivot, object element)
        {
            throw new NotImplementedException();
        }

        public Task<long> LLenAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> LPopAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<T> LPopAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<long> LPosAsync<T>(string key, T element, int rank = 0)
        {
            throw new NotImplementedException();
        }

        public Task<long[]> LPosAsync<T>(string key, T element, int rank, int count, int maxLen)
        {
            throw new NotImplementedException();
        }

        public Task<long> LPushAsync(string key, params object[] elements)
        {
            throw new NotImplementedException();
        }

        public Task<long> LPushXAsync(string key, params object[] elements)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> LRangeAsync(string key, long start, long stop)
        {
            throw new NotImplementedException();
        }

        public Task<T[]> LRangeAsync<T>(string key, long start, long stop)
        {
            throw new NotImplementedException();
        }

        public Task<long> LRemAsync<T>(string key, long count, T element)
        {
            throw new NotImplementedException();
        }

        public Task LSetAsync<T>(string key, long index, T element)
        {
            throw new NotImplementedException();
        }

        public Task LTrimAsync(string key, long start, long stop)
        {
            throw new NotImplementedException();
        }

        public Task<string> RPopAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<T> RPopAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> RPopLPushAsync(string source, string destination)
        {
            throw new NotImplementedException();
        }

        public Task<T> RPopLPushAsync<T>(string source, string destination)
        {
            throw new NotImplementedException();
        }

        public Task<long> RPushAsync(string key, params object[] elements)
        {
            throw new NotImplementedException();
        }

        public Task<long> RPushXAsync(string key, params object[] elements)
        {
            throw new NotImplementedException();
        }
    }
}