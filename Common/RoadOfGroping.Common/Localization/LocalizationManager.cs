namespace RoadOfGroping.Common.Localization
{
    /// <summary>
    /// 提供本地化管理功能的类，允许添加和获取本地化源。
    /// </summary>
    public class LocalizationManager : ILocalizationManager
    {
        private readonly Dictionary<string, ILocalizationSource> _sources;

        /// <summary>
        /// 构造函数，初始化本地化管理器并添加初始本地化源。
        /// </summary>
        /// <param name="localizationSource">要添加的本地化源。</param>
        public LocalizationManager(ILocalizationSource localizationSource)
        {
            // 确保传入的本地化源不为 null
            _sources = new Dictionary<string, ILocalizationSource>();
            if (localizationSource == null)
            {
                throw new ArgumentNullException(nameof(localizationSource), "本地化源不能为空。");
            }

            // 添加初始本地化源
            AddSource(localizationSource.Name, localizationSource);
        }

        /// <summary>
        /// 根据名称获取本地化源。
        /// </summary>
        /// <param name="name">本地化源的名称。</param>
        /// <returns>返回对应的 <see cref="ILocalizationSource"/>。</returns>
        /// <exception cref="ArgumentException">如果未找到对应的本地化源。</exception>
        public ILocalizationSource GetSource(string name)
        {
            if (_sources.TryGetValue(name, out var source))
            {
                return source;
            }
            throw new ArgumentException($"未能找到名为 '{name}' 的本地化资源。");
        }

        /// <summary>
        /// 获取所有已注册的本地化源。
        /// </summary>
        /// <returns>返回只读的本地化源列表。</returns>
        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return _sources.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// 添加本地化源。
        /// </summary>
        /// <param name="name">本地化源的名称。</param>
        /// <param name="source">要添加的本地化源。</param>
        /// <exception cref="ArgumentException">如果名称为空或已存在。</exception>
        /// <exception cref="ArgumentNullException">如果 source 为 null。</exception>
        public void AddSource(string name, ILocalizationSource source)
        {
            // 检查名称是否为 null 或空字符串
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("资源名称不能为空。", nameof(name));
            }

            // 检查 source 是否为 null
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "本地化资源不能为空。");
            }

            // 检查是否已经存在同名的资源
            if (_sources.ContainsKey(name))
            {
                throw new ArgumentException($"已存在名为 '{name}' 的本地化资源。", nameof(name));
            }

            // 添加本地化源
            _sources[name] = source;
        }
    }
}