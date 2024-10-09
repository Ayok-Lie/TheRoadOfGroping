using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RoadOfGroping.Common.Localization
{
    // 实现IStringLocalizerFactory接口的密封工厂类，用于创建JsonStringLocalizer
    public sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        // 用于缓存已创建的本地化器
        private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache = new();
        // 日志记录器
        private readonly ILogger _logger;
        // 本地化选项
        private readonly JsonLocalizationOptions _localizationOptions;

        // 构造函数，通过依赖注入获取选项和日志工厂
        public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> localizationOptions, ILoggerFactory loggerFactory)
        {
            // 获取本地化选项的值
            _localizationOptions = localizationOptions.Value;
            // 创建日志记录器
            _logger = loggerFactory.CreateLogger<JsonStringLocalizer>();
        }

        // 根据资源源类型创建本地化器
        public IStringLocalizer Create(Type resourceSource)
        {
            // 获取资源名称，去除前缀
            var resourceName = TrimPrefix(resourceSource.FullName, (_localizationOptions.RootNamespace ?? Assembly.GetEntryAssembly()?.GetName().Name ?? AppDomain.CurrentDomain.FriendlyName) + ".");
            return CreateJsonStringLocalizer(resourceName);
        }

        // 根据基本名称和位置创建本地化器
        public IStringLocalizer Create(string baseName, string location)
        {
            // 获取资源名称，去除前缀
            var resourceName = TrimPrefix(baseName, location + ".");
            return CreateJsonStringLocalizer(resourceName);
        }

        // 创建JsonStringLocalizer的实例
        private JsonStringLocalizer CreateJsonStringLocalizer(string resourceName)
        {
            // 记录查找资源的日志
            _logger.LogInformation("Looking for resource: {resourceName}", resourceName);
            // 使用资源名称获取或添加本地化器
            return _localizerCache.GetOrAdd(resourceName, resName => new JsonStringLocalizer(
                _localizationOptions,
                resName,
                _logger));
        }

        // 去除前缀的方法
        private static string TrimPrefix(string name, string prefix)
        {
            return name.StartsWith(prefix, StringComparison.Ordinal)
                    ? name.Substring(prefix.Length) // 返回去除前缀后的名称
                    : name; // 如果不以前缀开头，则返回原名称
        }
    }
}
