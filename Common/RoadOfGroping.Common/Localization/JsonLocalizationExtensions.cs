using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace RoadOfGroping.Common.Localization
{
    public static class JsonLocalizationExtensions
    {
        /// <summary>
        /// 添加应用程序所需的本地化服务。
        /// </summary>
        /// <param name="services">要添加服务的 <see cref="IServiceCollection"/>。</param>
        /// <returns>返回 <see cref="IServiceCollection"/> 以便可以链式调用其他方法。</returns>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
        {
            // 添加选项支持
            services.AddOptions();
            // 添加日志记录服务
            services.AddLogging();
            // 配置JsonLocalizationOptions，设置资源路径的默认值
            services.PostConfigure<JsonLocalizationOptions>(options => { options.ResourcesPath ??= "Resources"; });
            // 尝试注册IStringLocalizer泛型接口的实现
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            // 尝试注册IStringLocalizerFactory的单例实现
            services.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            // 返回服务集合以供后续调用
            return services;
        }

        /// <summary>
        /// 添加应用程序所需的本地化服务，并允许自定义配置。
        /// </summary>
        /// <param name="services">要添加服务的 <see cref="IServiceCollection"/>。</param>
        /// <param name="setupAction">
        /// 一个 <see cref="Action{LocalizationOptions}"/> 用于配置 <see cref="JsonLocalizationOptions"/>。
        /// </param>
        /// <returns>返回 <see cref="IServiceCollection"/> 以便可以链式调用其他方法。</returns>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptions> setupAction)
        {
            // 配置JsonLocalizationOptions
            services.Configure(setupAction);
            // 调用无参数的重载，完成服务的添加
            AddJsonLocalization(services);

            // 返回服务集合以供后续调用
            return services;
        }
    }
}