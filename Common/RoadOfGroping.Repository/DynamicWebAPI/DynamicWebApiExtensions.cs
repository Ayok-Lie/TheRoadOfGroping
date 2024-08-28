using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace RoadOfGroping.Repository.DynamicWebAPI
{
    /// <summary>
    /// 动态WebAPI扩展类，用于在ASP.NET Core应用程序中添加动态WebAPI功能。
    /// </summary>
    public static class DynamicWebApiExtensions    {
        /// <summary>
        /// 为IMvcBuilder添加动态WebAPI功能。
        /// </summary>
        /// <param name="builder">IMvcBuilder实例。</param>
        /// <returns>IMvcBuilder实例。</returns>
        public static IMvcBuilder AddDynamicWebApi(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // 配置应用程序部分管理器，添加自定义的控制器特性提供程序
            builder.ConfigureApplicationPartManager(applicationPartManager =>
            {
                applicationPartManager.FeatureProviders.Add(new ApplicationServiceControllerFeatureProvider());
            });

            // 配置MvcOptions，添加自定义的应用程序模型约定
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new ApplicationServiceConvention());
            });

            return builder;
        }

        /// <summary>
        /// 为IMvcCoreBuilder添加动态WebAPI功能。
        /// </summary>
        /// <param name="builder">IMvcCoreBuilder实例。</param>
        /// <returns>IMvcCoreBuilder实例。</returns>
        public static IMvcCoreBuilder AddDynamicWebApi(this IMvcCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // 配置应用程序部分管理器，添加自定义的控制器特性提供程序
            builder.ConfigureApplicationPartManager(applicationPartManager =>
            {
                applicationPartManager.FeatureProviders.Add(new ApplicationServiceControllerFeatureProvider());
            });

            // 配置MvcOptions，添加自定义的应用程序模型约定
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new ApplicationServiceConvention());
            });

            return builder;
        }
    }
}