using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RoadOfGroping.Common.Localization
{
    // 内部密封类，实现IStringLocalizer接口
    internal sealed class JsonStringLocalizer : IStringLocalizer
    {
        // 用于缓存资源的并发字典
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _resourcesCache = new();
        // 资源文件路径
        private readonly string _resourcesPath;
        // 资源名称
        private readonly string _resourceName;
        // 资源路径类型
        private readonly ResourcesPathType _resourcesPathType;
        // 日志记录器
        private readonly ILogger _logger;

        // 搜索的资源位置
        private string _searchedLocation;

        private string path;

        // 构造函数，初始化本地化器
        public JsonStringLocalizer(
            JsonLocalizationOptions localizationOptions,
            string resourceName,
            ILogger logger)
        {
            // 资源名称不能为空
            _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            // 日志记录器，如果为空则使用空日志记录器
            _logger = logger ?? NullLogger.Instance;
            // 资源路径，基于应用程序域的基目录和配置的资源路径
            _resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localizationOptions.ResourcesPath);

            // 使用当前执行程序集的目录和配置的资源路径来构造资源路径
            path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, localizationOptions.ResourcesPath);
            // 资源路径类型
            _resourcesPathType = localizationOptions.ResourcesPathType;
        }

        // 索引器，根据名称获取本地化字符串
        public LocalizedString this[string name]
        {
            get
            {
                // 安全地获取字符串值
                var value = GetStringSafely(name);
                // 返回本地化字符串，如果未找到资源，则使用名称本身
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _searchedLocation);
            }
        }

        // 索引器，根据名称和参数获取本地化字符串
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                // 安全地获取字符串值
                var format = GetStringSafely(name);
                // 格式化字符串
                var value = string.Format(format ?? name, arguments);
                // 返回本地化字符串，如果未找到资源，则使用名称本身
                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _searchedLocation);
            }
        }

        // 获取所有本地化字符串，包括父文化
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);

        // 根据文化获取本地化器（当前实现返回自身）
        public IStringLocalizer WithCulture(CultureInfo culture) => this;

        // 获取所有本地化字符串，包括父文化
        private IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            // 获取所有资源名称
            var resourceNames = includeParentCultures
                ? GetAllStringsFromCultureHierarchy(culture)
                : GetAllResourceStrings(culture);

            // 遍历资源名称，返回本地化字符串
            foreach (var name in resourceNames)
            {
                var value = GetStringSafely(name);
                yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _searchedLocation);
            }
        }

        // 安全地获取字符串值
        private string GetStringSafely(string name)
        {
            string value = null;

            // 获取当前文化对应的资源
            var resources = GetResources(CultureInfo.CurrentUICulture.Name);
            if (resources?.TryGetValue(name, out var resource) is true)
            {
                value = resource;
            }

            return value;
        }

        // 从文化层次结构中获取所有字符串
        private IEnumerable<string> GetAllStringsFromCultureHierarchy(CultureInfo startingCulture)
        {
            var currentCulture = startingCulture;
            var resourceNames = new HashSet<string>();

            // 遍历文化层次结构
            while (currentCulture.Equals(currentCulture.Parent) == false)
            {
                var cultureResourceNames = GetAllResourceStrings(currentCulture);

                if (cultureResourceNames != null)
                {
                    foreach (var resourceName in cultureResourceNames)
                    {
                        resourceNames.Add(resourceName);
                    }
                }

                currentCulture = currentCulture.Parent;
            }

            return resourceNames;
        }

        // 获取特定文化的所有资源字符串
        private IEnumerable<string> GetAllResourceStrings(CultureInfo culture)
        {
            var resources = GetResources(culture.Name);
            return resources?.Select(r => r.Key);
        }

        // 获取特定文化的资源
        private Dictionary<string, string> GetResources(string culture)
        {
            return _resourcesCache.GetOrAdd(culture, _ =>
            {
                var resourceFile = "json";
                if (_resourcesPathType == ResourcesPathType.TypeBased)
                {
                    resourceFile = $"{culture}.json";
                    if (_resourceName != null)
                    {
                        resourceFile = string.Join(".", _resourceName.Replace('.', Path.DirectorySeparatorChar), resourceFile);
                    }
                }
                else
                {
                    resourceFile = $"Language.{culture}.json";
                }

                _searchedLocation = Path.Combine(_resourcesPath, resourceFile);
                Dictionary<string, string> value = null;

                // 检查资源文件是否存在
                if (File.Exists(_searchedLocation))
                {
                    try
                    {
                        using var stream = File.OpenRead(_searchedLocation);
                        value = JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to get json content, path: {path}", _searchedLocation);
                    }
                }
                else
                {
                    _logger.LogWarning("Resource file {path} not exists", _searchedLocation);
                }

                return value;
            });
        }
    }
}
