using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            return _redisClient.SetAsync(key, value, GetExpireSeconds(expire));
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
    }
}
