using Microsoft.AspNetCore.SignalR;

namespace RoadOfGroping.Utility.MessageCenter.SignalR
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="Id">连接ID</param>
    /// <param name="User">用户名</param>
    /// <param name="Message">消息</param>
    public record TransData(string Id, string User, string Message);

    public interface IChatClient
    {
        Task ReceiveMessage(TransData data);
    }

    /// <summary>
    /// https://docs.microsoft.com/zh-cn/aspnet/core/signalr/hubs?view=aspnetcore-6.0
    /// https://blog.csdn.net/m0_37894611/article/details/125932989
    /// </summary>
    public class TestChatHub : Hub<IChatClient>
    {
        //用户集
        private static readonly Dictionary<string, string> _connections = new();

        private readonly string systemid = "system";
        private readonly string systemname = "system";

        #region 发送消息

        /// <summary>
        /// 以个人名义向所有客户端发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToAll(string message)
        {
            string cid = GetConnectionId();
            await Clients.All.ReceiveMessage(new(cid, _connections[cid], message));
        }

        /// <summary>
        /// 以系统名义向所有客户端发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendSysToAll(string message) => await Clients.All.ReceiveMessage(new(systemid, systemname, message));

        /// <summary>
        /// 发送消息给指定用户(个人)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToOne(string id, string message)
        {
            string cid = GetConnectionId();
            await Clients.Client(id).ReceiveMessage(new(cid, _connections[cid], message));
        }

        /// <summary>
        /// 发送消息给指定用户(系统)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendSysToOne(string id, string message) => await Clients.Client(id).ReceiveMessage(new(systemid, systemname, message));

        /// <summary>
        /// 发送群组消息(个人)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToGroup(string group, string message)
        {
            string cid = GetConnectionId();
            await Clients.Group(group).ReceiveMessage(new(cid, _connections[cid], message));
        }

        /// <summary>
        /// 发送群组消息(系统)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendSysToGroup(string group, string message) => await Clients.Group(group).ReceiveMessage(new(systemid, systemname, message));

        #endregion 发送消息

        #region SignalR用户

        /// <summary>
        /// 获取连接的唯一 ID（由 SignalR 分配）。 每个连接都有一个连接 ID
        /// </summary>
        /// <returns></returns>
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        #endregion SignalR用户

        #region SignalR群组

        /// <summary>
        /// 主动加入群组
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task AddToGroup(string group)
        {
            string cid = GetConnectionId();
            await Groups.AddToGroupAsync(cid, group);
            await SendSysToGroup(group, $@"欢迎{_connections[cid]}加入");
        }

        /// <summary>
        /// 被动加入群组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task AddToGrouped(string group, string id)
        {
            string cid = GetConnectionId();
            await Groups.AddToGroupAsync(id, group);
            await SendSysToGroup(group, $@"欢迎{_connections[cid]}加入");
        }

        #endregion SignalR群组

        #region 临时用户操作

        /// <summary>
        /// 添加到在线用户集
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task AddUser(string name)
        {
            string cid = GetConnectionId();
            if (!_connections.ContainsKey(cid))
            {
                await Task.Run(() => _connections.Add(cid, name));
                await SendSysToAll("relst");
            }
        }

        /// <summary>
        /// 获取在线用户
        /// </summary>
        /// <returns></returns>
        public object GetUser()
        {
            string cid = GetConnectionId();
            return _connections.Where(t => !t.Key.Equals(cid));
        }

        #endregion 临时用户操作

        #region 重写连接断开钩子

        /// <summary>
        /// 重写链接钩子
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 重写断开链接钩子
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string cid = GetConnectionId();
            _connections.Remove(cid);
            await SendSysToAll("relst");
            await base.OnDisconnectedAsync(exception);
        }

        #endregion 重写连接断开钩子
    }
}