using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.UserAppService
{
    public class RedisAppService : ApplicationService
    {
        private readonly ICacheTool cacheTool;

        public RedisAppService(ICacheTool cacheTool)
        {
            this.cacheTool = cacheTool;
        }

        public async Task SetRedis(string key ,string val)
        {
            await cacheTool.SetAsync(key, val);
        }

        public async Task<string> GetRedis(string key)
        {
            return await cacheTool.GetAsync(key);
        }
    }
}