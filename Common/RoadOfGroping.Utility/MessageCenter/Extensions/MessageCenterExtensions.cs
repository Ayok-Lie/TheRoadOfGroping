using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoadOfGroping.Utility.MessageCenter.AisMms;
using RoadOfGroping.Utility.MessageCenter.Dingtalk;
using RoadOfGroping.Utility.MessageCenter.Email;
using RoadOfGroping.Utility.MessageCenter.EnterpriseWeChat;
using RoadOfGroping.Utility.MessageCenter.IHuaWeiSms;
using RoadOfGroping.Utility.MessageCenter.Robot;
using RoadOfGroping.Utility.MessageCenter.TencentSms;

namespace RoadOfGroping.Utility.MessageCenter.Extensions
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