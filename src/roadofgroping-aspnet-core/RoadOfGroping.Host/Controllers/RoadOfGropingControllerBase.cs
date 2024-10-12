using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RoadOfGroping.Common.Localization;
using RoadOfGroping.Repository.UserSession;

namespace RoadOfGroping.Host.Controllers
{
    /// <summary>
    /// 基础控制器，所有特定控制器应该从这个控制器继承
    /// </summary>
    public class RoadOfGropingControllerBase : ControllerBase
    {
        /// <summary>
        /// 用户信息服务，通过它可以访问当前用户会话的信息
        /// </summary>
        public IUserSession UserService { get; set; }

        /// <summary>
        /// 对象映射器，用于对象之间的转换
        /// </summary>
        public IMapper ObjectMapper { get; set; }

        /// <summary>
        /// 本地化服务，用于获取地区特定的字符串
        /// </summary>
        public IStringLocalizer<AllLocalizationClass> Localizer { get; set; }

        /// <summary>
        /// 构造函数，通过服务提供者获取所需的服务
        /// </summary>
        /// <param name="serviceProvider">服务提供者，用于获取依赖项</param>
        public RoadOfGropingControllerBase(IServiceProvider serviceProvider)
        {
            // 从服务提供者中获取对象映射器
            ObjectMapper = serviceProvider.GetRequiredService<IMapper>();
            // 从服务提供者中获取用户会话服务
            UserService = serviceProvider.GetRequiredService<IUserSession>();
            // 从服务提供者中获取本地化服务
            Localizer = serviceProvider.GetRequiredService<IStringLocalizer<AllLocalizationClass>>();
        }

        /// <summary>
        /// 通过字符串名称获取本地化字符串
        /// </summary>
        /// <param name="name">字符串名称</param>
        /// <returns>对应的本地化字符串</returns>
        protected virtual string L(string name)
        {
            return Localizer.GetString(name);
        }

        /// <summary>
        /// 通过字符串名称和参数获取格式化的本地化字符串
        /// </summary>
        /// <param name="name">字符串名称</param>
        /// <param name="args">用于格式化的参数</param>
        /// <returns>格式化后的本地化字符串</returns>
        protected virtual string L(string name, params object[] args)
        {
            return Localizer.GetString(name, args);
        }
    }
}