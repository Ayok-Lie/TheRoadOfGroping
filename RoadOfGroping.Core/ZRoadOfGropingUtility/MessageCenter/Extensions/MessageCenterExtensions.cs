using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.AisMms;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Dingtalk;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Email;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.EnterpriseWeChat;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.IHuaWeiSms;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Robot;
using RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.TencentSms;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Extensions
{
    public static class MessageCenterExtensions
    {
        public static IServiceCollection AddMessageCenter(this IServiceCollection services)
        {
            services.TryAddTransient<IAiSmsManager, AiSmsManager>();
            services.TryAddTransient<ITencentSmsManager, TencentSmsManager>();
            services.TryAddTransient<IHuaWeiSmsManager, HuaWeiSmsManager>();
            services.TryAddTransient<IRobotPushManager, RobotPushManager>();
            services.TryAddTransient<IEmailPushManager, EmailPushManager>();
            services.TryAddTransient<IDingtalkManager, DingtalkManager>();
            services.TryAddTransient<IEnterpriseWeChatManager, EnterpriseWeChatManager>();
            services.AddHttpApi<IRobotPushHttpApi>();
            return services;
        }
    }
}