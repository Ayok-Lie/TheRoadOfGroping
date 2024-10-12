using System.Globalization;
using System.Text;
using Microsoft.Extensions.Localization;

namespace RoadOfGroping.Common.Localization
{
    /// <summary>
    /// 实现本地化源的类，允许获取本地化字符串。
    /// </summary>
    public class LocalizationSource : ILocalizationSource
    {
        public string Name { get; }
        private readonly IStringLocalizer _localizations;

        /// <summary>
        /// 构造函数，初始化本地化源。
        /// </summary>
        public LocalizationSource(string name, IStringLocalizerFactory factory, Type type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _localizations = factory?.Create(name, type.Name) ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// 获取本地化字符串，如果未找到则抛出异常。
        /// </summary>
        public string GetString(string name)
        {
            return GetStringOrNull(name) ?? throw new KeyNotFoundException($"Key '{name}' not found.");
        }

        /// <summary>
        /// 获取指定文化下的本地化字符串。
        /// </summary>
        public string GetString(string name, CultureInfo culture)
        {
            return GetStringOrNull(name, culture) ?? throw new KeyNotFoundException($"Key '{name}' for culture '{culture.Name}' not found.");
        }

        /// <summary>
        /// 获取本地化字符串，并进行格式化。
        /// </summary>
        public string GetString(string name, params object[] args)
        {
            return string.Format(GetString(name), args);
        }

        /// <summary>
        /// 获取指定文化下的本地化字符串，并进行格式化。
        /// </summary>
        public string GetString(string name, CultureInfo culture, params object[] args)
        {
            return string.Format(GetString(name, culture), args);
        }

        /// <summary>
        /// 获取本地化字符串，返回可能为 null。
        /// </summary>
        public string GetStringOrNull(string name, bool tryDefaults = true)
        {
            var value = _localizations.GetString(name).Value;
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (tryDefaults && CultureInfo.CurrentCulture != CultureInfo.InvariantCulture)
            {
                return GetStringOrNull($"{name}-{CultureInfo.CurrentCulture.Name}", false);
            }

            return null;
        }

        /// <summary>
        /// 获取指定文化下的本地化字符串，返回可能为 null。
        /// </summary>
        public string GetStringOrNull(string name, CultureInfo culture, bool tryDefaults = true)
        {
            var key = $"{name}-{culture.Name}";
            var value = _localizations.GetString(key).Value;
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (tryDefaults && culture != CultureInfo.InvariantCulture)
            {
                return GetStringOrNull(name);
            }

            return null;
        }

        /// <summary>
        /// 获取多个本地化字符串。
        /// </summary>
        public List<string> GetStrings(List<string> names)
        {
            var results = new List<string>();
            foreach (var name in names)
            {
                results.Add(GetString(name));
            }
            return results;
        }

        /// <summary>
        /// 获取指定文化下的多个本地化字符串。
        /// </summary>
        public List<string> GetStrings(List<string> names, CultureInfo culture)
        {
            var results = new List<string>();
            foreach (var name in names)
            {
                results.Add(GetString(name, culture));
            }
            return results;
        }

        /// <summary>
        /// 获取多个本地化字符串，返回可能为 null。
        /// </summary>
        public List<string> GetStringsOrNull(List<string> names, bool tryDefaults = true)
        {
            var results = new List<string>();
            foreach (var name in names)
            {
                results.Add(GetStringOrNull(name, tryDefaults));
            }
            return results;
        }

        /// <summary>
        /// 获取指定文化下的多个本地化字符串，返回可能为 null。
        /// </summary>
        public List<string> GetStringsOrNull(List<string> names, CultureInfo culture, bool tryDefaults = true)
        {
            var results = new List<string>();
            foreach (var name in names)
            {
                results.Add(GetStringOrNull(name, culture, tryDefaults));
            }
            return results;
        }

        /// <summary>
        /// 获取所有本地化字符串。
        /// </summary>
        public IReadOnlyList<LocalizedString> GetAllStrings(bool includeDefaults = true)
        {
            var results = new List<LocalizedString>();
            foreach (var pair in _localizations.GetAllStrings())
            {
                results.Add(new LocalizedString(pair.Value, ExtractCultureNameFromKey(pair.Name)));
            }
            return results;
        }

        /// <summary>
        /// 获取指定文化下的所有本地化字符串。
        /// </summary>
        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture, bool includeDefaults = true)
        {
            var results = new List<LocalizedString>();
            var cultureSuffix = $"-{culture.Name}";
            foreach (var pair in _localizations.GetAllStrings())
            {
                if (pair.Name.EndsWith(cultureSuffix) || (includeDefaults && !pair.Name.Contains("-")))
                {
                    results.Add(new LocalizedString(pair.Value, ExtractCultureNameFromKey(pair.Name)));
                }
            }
            return results;
        }

        /// <summary>
        /// 从键中提取文化名称。
        /// </summary>
        private string ExtractCultureNameFromKey(string key)
        {
            var parts = key.Split('-');
            return parts.Length > 1 ? parts[1] : string.Empty;
        }
    }
}