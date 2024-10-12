using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using RoadOfGroping.Common.Consts;

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

            // 配置 JsonLocalizationOptions，设置资源路径的默认值
            services.PostConfigure<JsonLocalizationOptions>(options =>
            {
                options.ResourcesPath ??= "Resources"; // 如果未提供资源路径，使用默认值
            });

            // 尝试注册 IStringLocalizer 泛型接口的实现
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            // 尝试注册 IStringLocalizerFactory 的单例实现
            services.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            // 返回服务集合以便后续调用
            return services;
        }

        /// <summary>
        /// 添加应用程序所需的本地化服务，并允许自定义配置。
        /// </summary>
        /// <param name="services">要添加服务的 <see cref="IServiceCollection"/>。</param>
        /// <param name="setupAction">
        /// 一个 <see cref="Action{JsonLocalizationOptions}"/> 用于配置 <see cref="JsonLocalizationOptions"/>。
        /// </param>
        /// <returns>返回 <see cref="IServiceCollection"/> 以便可以链式调用其他方法。</returns>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services)); // 更改为抛出 ArgumentNullException
            }

            // 配置 JsonLocalizationOptions
            services.Configure(setupAction);

            // 调用无参数的重载，完成服务的添加
            AddJsonLocalization(services);

            // 注册 LocalizationSource 作为单例
            services.AddSingleton<ILocalizationSource, LocalizationSource>((serviceProvider) =>
            {
                var factory = serviceProvider.GetRequiredService<IStringLocalizerFactory>();
                return new LocalizationSource(RoadOfGropingConst.LocalizationSourceName, factory, typeof(AllLocalizationClass));
            });

            // 注册 LocalizationManager 作为单例
            services.AddSingleton<ILocalizationManager, LocalizationManager>();

            // 返回服务集合以便后续调用
            return services;
        }
    }
}