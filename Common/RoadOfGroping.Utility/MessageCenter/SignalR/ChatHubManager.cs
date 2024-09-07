using Microsoft.AspNetCore.SignalR;
using RoadOfGroping.Common.Consts;
using RoadOfGroping.Utility.MessageCenter.SignalR.Dtos;

namespace RoadOfGroping.Utility.MessageCenter.SignalR
{
    public class ChatHubManager : IChatHubManager
    {
        private readonly IHubContext<ChatHub, IChatService> _hubContext;
        private readonly ICacheTool _cacheManager;

        public ChatHubManager(IHubContext<ChatHub, IChatService> hubContext,
            ICacheTool cacheManager)
        {
            _hubContext = hubContext;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 所有连接的id发送消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SendAll(MessageInput input)
        {
            await _hubContext.Clients.All.ReceiveMessage(input);
        }

        public async Task SendOtherUser(MessageInput input)
        {
            var connectionClients = await _cacheManager.LRangeAsync<ConnectionClient>(CacheConst.SignlRKey, 0, -1);
            var hubClient = connectionClients.FirstOrDefault(p => p.GroupName == (input.UserId));
            if (hubClient != null)
            {
                await _hubContext.Clients.AllExcept(hubClient.ConnectionId).ReceiveMessage(input);
            }
        }

        /// <summary>
        /// 当前用户多个地方登录在线
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SendUser(MessageInput input)
        {
            var connectionClients = await _cacheManager.LRangeAsync<ConnectionClient>(CacheConst.SignlRKey, 0, -1);
            var hubClients = connectionClients.Where(p => p.GroupName == input.UserId);
            if (!hubClients.Any()) return;
            var connectionIds = hubClients.Select(c => c.ConnectionId).ToList();
            await _hubContext.Clients.Clients(connectionIds).ReceiveMessage(input);
        }

        public async Task SendUsers(MessageInput input)
        {
            var connectionClients = await _cacheManager.LRangeAsync<ConnectionClient>(CacheConst.SignlRKey, 0, -1);
            var hubClients = connectionClients.Where(p => input.UserIds.Contains(p.GroupName));
            if (!hubClients.Any()) return;
            var connectionIds = hubClients.Select(c => c.ConnectionId).ToList();
            await _hubContext.Clients.Clients(connectionIds).ReceiveMessage(input);
        }
    }
}