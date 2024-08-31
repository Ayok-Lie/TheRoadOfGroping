using Microsoft.Extensions.Caching.Memory;

namespace RoadOfGroping.Utility.RedisModule
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
    }
}