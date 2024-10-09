using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Core.ZRoadOfGropingUtility.RedisModule;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    /// <summary>
    /// redis测试服务
    /// </summary>
    [DisabledUnitOfWork(true)]
    public class RedisAppService : ApplicationService
    {
        private readonly CacheManager cacheTool;

        public RedisAppService(IServiceProvider serviceProvider, CacheManager cacheTool) : base(serviceProvider)
        {
            this.cacheTool = cacheTool;
        }

        /// <summary>
        /// 设置redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task SetRedis(string key, string val)
        {
            await cacheTool.SetAsync(key, val);
        }

        public async Task<string> GetRedis(string key)
        {
            return await cacheTool.GetAsync(key);
        }
    }
}