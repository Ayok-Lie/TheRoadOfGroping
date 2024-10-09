using FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.RedisModule
{
    public class CacheManager : ITransientDependency
    {
        private readonly ICacheTool _redisCacheTool;
        private readonly RedisCacheOptions options;
        private readonly IServiceProvider serviceProvider;

        public CacheManager(IOptions<RedisCacheOptions> option, IServiceProvider serviceProvider)
        {
            this.options = option.Value;
            this.serviceProvider = serviceProvider;
            if (options.EnableRedis)
            {
                _redisCacheTool = serviceProvider.GetRequiredService<RedisCacheTool>();
            }
            else
            {
                _redisCacheTool = serviceProvider.GetRequiredService<MemoryCacheTool>();
            }
        }

        public void Set(string key, object value) => _redisCacheTool.Set(key, value);

        public Task SetAsync(string key, object value, TimeSpan? expire = null)
            => _redisCacheTool.SetAsync(key, value, expire);

        public string Get(string key) => _redisCacheTool.Get(key);

        public T Get<T>(string key) => _redisCacheTool.Get<T>(key);

        public Task<string> GetAsync(string key) => _redisCacheTool.GetAsync(key);

        public Task<T> GetAsync<T>(string key) => _redisCacheTool.GetAsync<T>(key);

        public long Del(params string[] keys) => _redisCacheTool.Del(keys);

        public Task<long> DelAsync(params string[] keys) => _redisCacheTool.DelAsync(keys);

        public bool Exists(string key) => _redisCacheTool.Exists(key);

        public Task<bool> ExistsAsync(string key) => _redisCacheTool.ExistsAsync(key);

        public long LLen(string key) => _redisCacheTool.LLen(key);

        public Task<long> LLenAsync(string key) => _redisCacheTool.LLenAsync(key);

        public string LPop(string key) => _redisCacheTool.LPop(key);

        public Task<string> LPopAsync(string key) => _redisCacheTool.LPopAsync(key);

        public string RPop(string key) => _redisCacheTool.RPop(key);

        public Task<string> RPopAsync(string key) => _redisCacheTool.RPopAsync(key);

        public long RPush(string key, params object[] elements) => _redisCacheTool.RPush(key, elements);

        public Task<long> RPushAsync(string key, params object[] elements) => _redisCacheTool.RPushAsync(key, elements);

        public async Task RemoveByPrefixAsync(string key) => await _redisCacheTool.RemoveByPrefixAsync(key);

        public void Set(string key, object value, TimeSpan expire) => _redisCacheTool.Set(key, value, expire);

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expire = null)
            => await _redisCacheTool.GetOrSetAsync(key, func, expire);

        public string BLPop(string key, int timeoutSeconds) => _redisCacheTool.BLPop(key, timeoutSeconds);

        public async Task<string> BLPopAsync(string key, int timeoutSeconds) => await _redisCacheTool.BLPopAsync(key, timeoutSeconds);

        public string BRPop(string key, int timeoutSeconds) => _redisCacheTool.BRPop(key, timeoutSeconds);

        public async Task<string> BRPopAsync(string key, int timeoutSeconds) => await _redisCacheTool.BRPopAsync(key, timeoutSeconds);

        public string[] LRange(string key, long start, long stop) => _redisCacheTool.LRange(key, start, stop);

        public async Task<string[]> LRangeAsync(string key, long start, long stop) => await _redisCacheTool.LRangeAsync(key, start, stop);

        public long LInsert(string key, InsertDirection direction, object pivot, object element) => _redisCacheTool.LInsert(key, direction, pivot, element);

        public async Task<long> LInsertAsync(string key, InsertDirection direction, object pivot, object element) => await _redisCacheTool.LInsertAsync(key, direction, pivot, element);

        public long LRem<T>(string key, long count, T element) => _redisCacheTool.LRem(key, count, element);

        public async Task<long> LRemAsync<T>(string key, long count, T element) => await _redisCacheTool.LRemAsync(key, count, element);

        public void LSet<T>(string key, long index, T element) => _redisCacheTool.LSet(key, index, element);

        public async Task LSetAsync<T>(string key, long index, T element) => await _redisCacheTool.LSetAsync(key, index, element);

        public void LTrim(string key, long start, long stop) => _redisCacheTool.LTrim(key, start, stop);

        public async Task LTrimAsync(string key, long start, long stop) => await _redisCacheTool.LTrimAsync(key, start, stop);

        public long LPos<T>(string key, T element, int rank = 0) => _redisCacheTool.LPos(key, element, rank);

        public async Task<long> LPosAsync<T>(string key, T element, int rank = 0) => await _redisCacheTool.LPosAsync(key, element, rank);

        public T BLPop<T>(string key, int timeoutSeconds)
        {
            return _redisCacheTool.BLPop<T>(key, timeoutSeconds);
        }

        public KeyValue<string> BLPop(string[] keys, int timeoutSeconds)
        {
            return _redisCacheTool.BLPop(keys, timeoutSeconds);
        }

        public KeyValue<T> BLPop<T>(string[] keys, int timeoutSeconds)
        {
            return _redisCacheTool.BLPop<T>(keys, timeoutSeconds);
        }

        public T BRPop<T>(string key, int timeoutSeconds)
        {
            return _redisCacheTool.BRPop<T>(key, timeoutSeconds);
        }

        public KeyValue<string> BRPop(string[] keys, int timeoutSeconds)
        {
            return _redisCacheTool.BRPop(keys, timeoutSeconds);
        }

        public KeyValue<T> BRPop<T>(string[] keys, int timeoutSeconds)
        {
            return _redisCacheTool.BRPop<T>(keys, timeoutSeconds);
        }

        public string BRPopLPush(string source, string destination, int timeoutSeconds)
        {
            return _redisCacheTool.BRPopLPush(source, destination, timeoutSeconds);
        }

        public T BRPopLPush<T>(string source, string destination, int timeoutSeconds)
        {
            return _redisCacheTool.BRPopLPush<T>(source, destination, timeoutSeconds);
        }

        public string LIndex(string key, long index)
        {
            return _redisCacheTool.LIndex(key, index);
        }

        public T LIndex<T>(string key, long index)
        {
            return _redisCacheTool.LIndex<T>(key, index);
        }

        public T LPop<T>(string key)
        {
            return _redisCacheTool.LPop<T>(key);
        }

        public long[] LPos<T>(string key, T element, int rank, int count, int maxLen)
        {
            return _redisCacheTool.LPos<T>(key, element, rank, count, maxLen);
        }

        public long LPush(string key, params object[] elements)
        {
            return _redisCacheTool.LPush(key, elements);
        }

        public long LPushX(string key, params object[] elements)
        {
            return _redisCacheTool.LPushX(key, elements);
        }

        public T[] LRange<T>(string key, long start, long stop)
        {
            return _redisCacheTool.LRange<T>(key, start, stop);
        }

        public T RPop<T>(string key)
        {
            return _redisCacheTool.RPop<T>(key);
        }

        public string RPopLPush(string source, string destination)
        {
            return _redisCacheTool.RPopLPush(source, destination);
        }

        public T RPopLPush<T>(string source, string destination)
        {
            return _redisCacheTool.RPopLPush<T>(source, destination);
        }

        public long RPushX(string key, params object[] elements)
        {
            return _redisCacheTool.RPushX(key, elements);
        }

        public Task<T> BLPopAsync<T>(string key, int timeoutSeconds)
        {
            return _redisCacheTool.BLPopAsync<T>(key, timeoutSeconds);
        }

        public Task<KeyValue<string>> BLPopAsync(string[] keys, int timeoutSeconds)
        {
            return _redisCacheTool.BLPopAsync(keys, timeoutSeconds);
        }

        public Task<KeyValue<T>> BLPopAsync<T>(string[] keys, int timeoutSeconds)
        {
            return _redisCacheTool.BLPopAsync<T>(keys, timeoutSeconds);
        }

        public Task<T> BRPopAsync<T>(string key, int timeoutSeconds)
        {
            return _redisCacheTool.BRPopAsync<T>(key, timeoutSeconds);
        }

        public Task<KeyValue<string>> BRPopAsync(string[] keys, int timeoutSeconds)
        {
            return _redisCacheTool.BRPopAsync(keys, timeoutSeconds);
        }

        public Task<KeyValue<T>> BRPopAsync<T>(string[] keys, int timeoutSeconds)
        {
            return _redisCacheTool.BRPopAsync<T>(keys, timeoutSeconds);
        }

        public Task<string> BRPopLPushAsync(string source, string destination, int timeoutSeconds)
        {
            return _redisCacheTool.BRPopLPushAsync(source, destination, timeoutSeconds);
        }

        public Task<T> BRPopLPushAsync<T>(string source, string destination, int timeoutSeconds)
        {
            return _redisCacheTool.BRPopLPushAsync<T>(source, destination, timeoutSeconds);
        }

        public Task<string> LIndexAsync(string key, long index)
        {
            return _redisCacheTool.LIndexAsync(key, index);
        }

        public Task<T> LIndexAsync<T>(string key, long index)
        {
            return _redisCacheTool.LIndexAsync<T>(key, index);
        }

        public Task<T> LPopAsync<T>(string key)
        {
            return _redisCacheTool.LPopAsync<T>(key);
        }

        public Task<long[]> LPosAsync<T>(string key, T element, int rank, int count, int maxLen)
        {
            return _redisCacheTool.LPosAsync<T>(key, element, rank, count, maxLen);
        }

        public Task<long> LPushAsync(string key, params object[] elements)
        {
            return _redisCacheTool.LPushAsync(key, elements);
        }

        public Task<long> LPushXAsync(string key, params object[] elements)
        {
            return _redisCacheTool.LPushXAsync(key, elements);
        }

        public Task<T[]> LRangeAsync<T>(string key, long start, long stop)
        {
            return _redisCacheTool.LRangeAsync<T>(key, start, stop);
        }

        public Task<T> RPopAsync<T>(string key)
        {
            return _redisCacheTool.RPopAsync<T>(key);
        }

        public Task<string> RPopLPushAsync(string source, string destination)
        {
            return _redisCacheTool.RPopLPushAsync(source, destination);
        }

        public Task<T> RPopLPushAsync<T>(string source, string destination)
        {
            return _redisCacheTool.RPopLPushAsync<T>(source, destination);
        }

        public Task<long> RPushXAsync(string key, params object[] elements)
        {
            return _redisCacheTool.RPushXAsync(key, elements);
        }
    }
}